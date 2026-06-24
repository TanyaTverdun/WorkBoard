using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler 
    : IRequestHandler<CreateCardCommand, CardDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public CreateCardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<CardDto> Handle(
        CreateCardCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var section = await uow.SectionRepository.GetByIdAsync(
            request.SectionId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {request.SectionId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            section.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to create cards on this board.");
        }

        var newCard = new Card
        {
            Id = Guid.NewGuid(),
            SectionId = request.SectionId,
            Title = request.Title,
            Position = request.Position,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        try
        {
            await uow.CardRepository.CreateAsync(newCard);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return new CardDto
        {
            Id = newCard.Id,
            SectionId = newCard.SectionId,
            Title = newCard.Title,
            Description = newCard.Description,
            DueDate = newCard.DueDate,
            Position = newCard.Position
        };
    }
}
