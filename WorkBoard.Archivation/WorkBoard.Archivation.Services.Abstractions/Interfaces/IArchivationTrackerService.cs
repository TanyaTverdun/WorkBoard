namespace WorkBoard.Archivation.Services.Abstractions.Interfaces;

public interface IArchivationTrackerService
{
    Task TrackStatusAsync(
        Guid boardId, 
        string status, 
        string? details = null);
}
