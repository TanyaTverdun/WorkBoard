namespace ArchivationFunctionApp.Interfaces;

public interface IArchivationTrackerService
{
    Task TrackStatusAsync(
        Guid boardId, 
        string status, 
        string? details = null);
}
