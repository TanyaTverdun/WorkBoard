using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Board;

public class BoardDto
{
    public Guid Id { get; set; }
    public Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
    public BoardRole UserRole { get; set; }
}
