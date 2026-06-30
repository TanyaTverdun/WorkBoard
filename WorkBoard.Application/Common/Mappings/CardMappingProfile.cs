using AutoMapper;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        CreateMap<Card, CardDto>();
    }
}
