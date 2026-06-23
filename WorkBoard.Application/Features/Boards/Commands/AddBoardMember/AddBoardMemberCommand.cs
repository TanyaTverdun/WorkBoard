using MediatR;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.AddBoardMember;

public record AddBoardMemberCommand(
    Guid BoardId,
    Guid TargetUserId,
    BoardRole Role) : IRequest<Unit>;
