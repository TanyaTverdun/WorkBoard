using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(Guid BoardId) : IRequest;
