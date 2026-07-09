using AutoMapper;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class AttachmentMappingProfile : Profile
{
    public AttachmentMappingProfile()
    {
        CreateMap<Attachment, AttachmentDto>();
    }
}
