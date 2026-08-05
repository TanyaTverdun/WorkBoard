namespace WorkBoard.Application.Common.Dtos.Boards;

public class BoardArchiveStatusUpdatedDto
{
    public Guid BoardId { get; set; }
    public bool IsArchived { get; set; }
}
