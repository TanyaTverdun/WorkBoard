namespace WorkBoard.Archivation.Services.Abstractions.Interfaces;

public interface IBlobArchivationService
{
    Task UploadArchiveAsync(
        string fileName, 
        string jsonContent);
}