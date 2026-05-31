using AutoMapper;
using WorkBoard.Application.Features.User.Commands.RegisterUser;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

/// <summary>
/// Configures AutoMapper mapping rules 
/// for the <see cref="User"/> entity
/// </summary>
public class UserMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UserMappingProfile"/> class and defines mappings
    /// </summary>
    public UserMappingProfile()
    {
        CreateMap<RegisterUserCommand, User>()
            .ForMember(
                dest => dest.AvatarUrl, 
                opt => opt.Ignore());
    }
}
