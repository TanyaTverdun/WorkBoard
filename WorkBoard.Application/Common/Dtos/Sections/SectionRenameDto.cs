namespace WorkBoard.Application.Common.Dtos.Sections;

public record SectionRenameDto(
    Guid SectionId, 
    string NewName);
