using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Checklists.Commands.CreateChecklist;

public class CreateChecklistCommandHandler
: IRequestHandler<CreateChecklistCommand, ChecklistDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public CreateChecklistCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChecklistDto> Handle(
        CreateChecklistCommand request,
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

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId,
            currentUserId,
            cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to modify this card.");
        }

        var checklist = new Checklist
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        try
        {
            await uow.ChecklistRepository.CreateAsync(
                checklist, 
                cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return _mapper.Map<ChecklistDto>(checklist);
    }
}
