using MediatR;

namespace WorkBoard.Application.Features.Attachments.Commands.DeleteAttachment;

public class DeleteAttachmentCommand : IRequest
{
    public Guid CardId { get; set; }
    public Guid AttachmentId { get; set; }
}
