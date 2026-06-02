using AutoMapper;
using WorkBoard.Application.Features.User.Commands.RegisterUser;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterUserCommand, User>()
            .ForMember(
                dest => dest.AvatarUrl, 
                opt => opt.Ignore());

        CreateMap<RegisterUserCommand, User>()
            .ForMember(
                dest => dest.Id, 
                opt => opt.MapFrom(src => src.UserId));
    }
}
