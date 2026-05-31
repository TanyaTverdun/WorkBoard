using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.User.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance 
    /// of the <see cref="UpdateUserProfileCommandHandler"/>
    /// </summary>
    /// <param name="userRepository">
    /// The repository for user data operations
    /// </param>
    /// <param name="unitOfWork">
    /// The unit of work to manage transactions
    /// </param>
    /// <param name="mapper">
    /// The AutoMapper instance for object mapping
    /// </param>
    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the profile update request
    /// </summary>
    /// <param name="request">
    /// The update command containing new profile data
    /// .</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation
    /// </param>
    /// <returns>
    /// True if the profile was successfully updated; 
    /// otherwise, false
    /// </returns>
    public async Task<bool> Handle(
        UpdateUserProfileCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            request.UserId, 
            cancellationToken);

        if (user == null)
        {
            return false;
        }

        _mapper.Map(request, user);

        _unitOfWork.BeginTransaction();

        var result = await _userRepository.UpdateProfileAsync(
            user, 
            cancellationToken);

        if (result)
        {
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }

        return false;
    }
}
