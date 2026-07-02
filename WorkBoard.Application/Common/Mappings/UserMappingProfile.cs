using AutoMapper;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Features.User.Commands.RegisterUser;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<AuthUserCommand, User>()
            .ForMember(
                dest => dest.Id, 
                opt => opt.MapFrom(src => src.UserId))

            .ForMember(
                dest => dest.AvatarUrl,
                opt => opt.Ignore());

        CreateMap<User, UserSearchDto>()
            .ForMember(
                dest => dest.UserId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(
                dest => dest.Initials,
                opt => opt.Ignore()
            );
    }
}
