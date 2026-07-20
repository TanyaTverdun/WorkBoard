using AutoMapper;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Models;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        CreateMap<Card, CardDto>()
            .ForMember(
                dest => dest.CommentsCount, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.AttachmentsCount, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ChecklistTotalItems, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.ChecklistDoneItems, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Labels, 
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Assignees, 
                opt => opt.Ignore());

        CreateMap<CardSummaryModel, CardDto>();

        CreateMap<LabelModel, LabelDto>();

        CreateMap<AssigneeModel, CardAssigneeDto>()
            .ForMember(
                dest => dest.Initials, 
                opt => opt.Ignore());

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
