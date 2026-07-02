using AutoMapper;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class ChecklistMappingProfile : Profile
{
    public ChecklistMappingProfile()
    {
        CreateMap<Checklist, ChecklistDto>()
            .ForMember(
                dest => dest.ChecklistId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.Items,
                opt => opt.Ignore());
    }
}
