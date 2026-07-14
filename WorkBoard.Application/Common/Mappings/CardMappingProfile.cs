using AutoMapper;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        CreateMap<Card, CardDto>();

        CreateMap<User, CardAssigneeDto>()
            .ForMember(
                dest => dest.UserId, 
                opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.Initials, 
                opt => opt.Ignore()
            );
    }
}
