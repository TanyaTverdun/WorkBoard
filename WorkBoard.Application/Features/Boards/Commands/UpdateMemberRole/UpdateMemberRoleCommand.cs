using MediatR;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateMemberRole;

public record UpdateMemberRoleCommand(
    Guid BoardId,
    Guid TargetUserId,
    BoardRole NewRole) : IRequest<Unit>;
