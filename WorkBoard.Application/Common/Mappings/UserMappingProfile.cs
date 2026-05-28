using AutoMapper;
using WorkBoard.Application.Common.Dtos.User;
using WorkBoard.Application.Features.User.Commands.UpdateUserProfile;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

/// <summary>
/// Configures AutoMapper profiles for <see cref="User"/> 
/// and its DTOs or Commands
/// </summary>
public class UserMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserMappingProfile"/> 
    /// class and defines mapping rules
    /// </summary>
    public UserMappingProfile()
    {
        /// <summary>
        /// Maps <see cref="UpdateUserProfileCommand"/> 
        /// to the <see cref="User"/> domain entity
        /// Used when updating profile details from the UI
        /// </summary>
        CreateMap<UpdateUserProfileCommand, User>();

        /// <summary>
        /// Maps the <see cref="User"/> domain entity to <see cref="UserDto"/>
        /// Used when returning user details back
        /// </summary>
        CreateMap<User, UserDto>();
    }
}
