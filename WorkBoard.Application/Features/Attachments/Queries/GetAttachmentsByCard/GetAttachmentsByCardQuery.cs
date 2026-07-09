using MediatR;
using WorkBoard.Application.Common.Dtos.Attachments;

namespace WorkBoard.Application.Features.Attachments.Queries.GetAttachmentsByCard;

public class GetAttachmentsByCardQuery 
    : IRequest<IReadOnlyList<AttachmentDto>>
{
    public Guid CardId { get; set; }
}
