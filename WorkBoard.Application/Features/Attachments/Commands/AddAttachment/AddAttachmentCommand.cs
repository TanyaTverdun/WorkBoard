using MediatR;
using WorkBoard.Application.Common.Dtos.Attachments;

namespace WorkBoard.Application.Features.Attachments.Commands.AddAttachment;

public class AddAttachmentCommand : IRequest<AttachmentDto>
{
    public Guid CardId { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public required Stream FileStream { get; set; }
}
