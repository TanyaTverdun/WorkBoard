using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Labels.Commands.CreateLabel;

public class CreateLabelCommandHandler
: IRequestHandler<CreateLabelCommand, LabelDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public CreateLabelCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<LabelDto> Handle(
        CreateLabelCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {request.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            section.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to create labels on this board.");
        }

        var isNameUnique = await uow.LabelRepository.IsNameUniqueAsync(
            section.BoardId,
            request.Name,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new InvalidOperationException(
                $"Label with name '{request.Name}' already exists on this board.");
        }

        var newLabelId = Guid.NewGuid();

        var newLabel = new Label
        {
            Id = newLabelId,
            BoardId = section.BoardId,
            Name = request.Name,
            Color = request.Color,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        try
        {
            await uow.LabelRepository.CreateAsync(
                newLabel,
                cancellationToken);

            await uow.CardLabelRepository.AddAsync(
                request.CardId,
                newLabelId,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return _mapper.Map<LabelDto>(newLabel);
    }
}
