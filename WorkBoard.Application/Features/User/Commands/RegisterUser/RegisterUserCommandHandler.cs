using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Interfaces;
using UserEntity = WorkBoard.Domain.Entities.User;

namespace WorkBoard.Application.Features.User.Commands.RegisterUser;

/// <summary>
/// Handles the business logic for registering a user authenticated
/// </summary>
public class RegisterUserCommandHandler 
    : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="RegisterUserCommandHandler"/> class
    /// </summary>
    /// <param name="userRepository">
    /// The repository to manage user data storage
    /// </param>
    /// <param name="unitOfWork">
    /// The unit of work to manage database transactions
    /// </param>
    /// <param name="mapper">
    /// The mapper to convert DTOs to entities
    /// </param>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the user registration request
    /// </summary>
    /// <param name="request">
    /// The registration command containing authenticated user details
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation
    /// </param>
    /// <returns>
    /// The indetifire <see cref="Guid"/> of the registered or existing user
    /// </returns>
    public async Task<Guid> Handle(
        RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository
            .GetByIdAsync(
                request.UserId, 
                cancellationToken);

        if (existingUser != null)
        {
            return existingUser.UserId;
        }

        var user = _mapper.Map<UserEntity>(request);

        _unitOfWork.BeginTransaction();

        try
        {
            await _userRepository.AddAsync(
                user, 
                _unitOfWork.CurrentTransaction, 
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            throw;
        }

        return user.UserId;
    }
}
