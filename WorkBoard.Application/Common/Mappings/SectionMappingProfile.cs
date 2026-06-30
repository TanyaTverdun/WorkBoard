using AutoMapper;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class SectionMappingProfile : Profile
{
    public SectionMappingProfile()
    {
        CreateMap<Section, SectionDto>();
    }
}
