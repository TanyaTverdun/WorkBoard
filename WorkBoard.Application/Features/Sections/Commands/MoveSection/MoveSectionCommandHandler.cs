using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Sections.Commands.MoveSection;

public class MoveSectionCommandHandler 
    : IRequestHandler<MoveSectionCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public MoveSectionCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        MoveSectionCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            request.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to reorder sections on this board.");
        }

        var section = await uow.SectionRepository.GetByIdAsync(
            request.SectionId, 
            cancellationToken);

        if (section == null || section.BoardId != request.BoardId)
        {
            throw new NotFoundException(
                $"Section with ID {request.SectionId} was not found on this board.");
        }

        section.Position = request.NewPosition;

        try
        {
            await uow.SectionRepository.UpdateAsync(section, cancellationToken);
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
