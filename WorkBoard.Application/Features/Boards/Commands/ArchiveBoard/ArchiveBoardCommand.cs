using MediatR;

namespace WorkBoard.Application.Features.Boards.Commands.ArchiveBoard;

public record ArchiveBoardCommand(Guid BoardId) : IRequest;
