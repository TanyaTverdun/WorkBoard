namespace WorkBoard.Application.Common.Dtos.Attachments;

public record AttachmentDeletedDto(
    Guid CardId, 
    Guid AttachmentId);
