using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.CreateBoard;

public record CreateBoardCommand(
    Guid WorkspaceId, 
    string Name) : IRequest<Guid>;
