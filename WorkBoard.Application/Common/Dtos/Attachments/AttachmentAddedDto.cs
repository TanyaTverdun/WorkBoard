namespace WorkBoard.Application.Common.Dtos.Attachments;

public record AttachmentAddedDto(
    Guid CardId, 
    AttachmentDto Attachment);
