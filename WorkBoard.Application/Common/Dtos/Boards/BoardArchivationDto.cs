using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Boards;

public class BoardArchivationDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string WorkspaceName { get; set; }
    public BoardArchiveStatus ArchiveStatus { get; set; }
}
