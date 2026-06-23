using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.RemoveBoardMember;

public record RemoveBoardMemberCommand(
    Guid BoardId,
    Guid TargetUserId) : IRequest<Unit>;
