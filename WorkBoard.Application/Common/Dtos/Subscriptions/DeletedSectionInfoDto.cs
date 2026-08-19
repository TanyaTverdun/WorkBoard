namespace WorkBoard.Application.Common.Dtos.Subscriptions;

public record DeletedSectionInfo(
    Guid BoardId, 
    Guid SectionId);
