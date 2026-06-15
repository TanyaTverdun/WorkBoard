using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateBoard;

public record UpdateBoardCommand(
    Guid BoardId, 
    string Name) : IRequest;
