using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.User.Queries.GetCurrentUserProfile;

public class GetCurrentUserProfileQueryHandler
    : IRequestHandler<GetCurrentUserProfileQuery, UserProfileDto>
{
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetCurrentUserProfileQueryHandler(
        IUserContext userContext,
        IMapper mapper)
    {
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated or not found.");

        var userDto = _mapper.Map<UserProfileDto>(user);
        userDto.Initials = InitialGenerator.Generate(user.FullName);

        return userDto;
    }
}
