namespace WorkBoard.Archivation.Domain.DTOs;

public class BoardMemberArchiveDto
{
    public Guid UserId { get; set; }
    public byte UserRole { get; set; }
}
