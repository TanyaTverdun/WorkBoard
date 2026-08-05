using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.RestoreBoard;

public record RestoreBoardCommand(Guid BoardId) : IRequest;
