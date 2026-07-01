using AutoMapper;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class LabelMappingProfile : Profile
{
    public LabelMappingProfile()
    {
        CreateMap<Label, LabelDto>();
    }
}
