using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Labels.Commands.AddLabelToCard;

public class AddLabelToCardCommandHandler
: IRequestHandler<AddLabelToCardCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public AddLabelToCardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        AddLabelToCardCommand request,
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
                "You do not have permission to modify labels on this card.");
        }

        var label = await uow.LabelRepository.GetByIdAsync(
            request.LabelId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Label with ID {request.LabelId} was not found.");

        if (label.BoardId != section.BoardId)
        {
            throw new InvalidOperationException(
                "Cannot attach a label from a different board.");
        }

        var isAlreadyAttached = await uow.CardLabelRepository.HasLabelAsync(
            request.CardId,
            request.LabelId,
            cancellationToken);

        if (isAlreadyAttached)
        {
            return Unit.Value;
        }

        try
        {
            await uow.CardLabelRepository.AddAsync(
                request.CardId,
                request.LabelId,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return Unit.Value;
    }
}
