using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using UserEntity = WorkBoard.Domain.Entities.User;

namespace WorkBoard.Application.Features.User.Commands.RegisterUser;

public class AuthUserCommandHandler 
    : IRequestHandler<AuthUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    private static readonly string[] AvatarColors = new[]
    {
        "#3b82f6",
        "#8b5cf6",
        "#10b981",
        "#f59e0b",
        "#ef4444",
        "#ec4899",
        "#84cc16",
        "#475569",
    };

    public AuthUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWorkFactory unitOfWorkFactory,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(
        AuthUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByIdOrEmailAsync(
            request.UserId,
            request.Email,
            cancellationToken);

        if (existingUser != null)
        {
            return existingUser.Id;
        }

        var user = _mapper.Map<UserEntity>(request);
        user.AvatarColor = AvatarColors[new Random().Next(AvatarColors.Length)];

        using var uow = _unitOfWorkFactory.Create();

        try
        {
            await uow.UserRepository.CreateAsync(
                user, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return user.Id;
    }
}
