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
        var existingUser = await _userRepository.GetByIdAsync(
            request.UserId, 
            cancellationToken);

        if (existingUser != null)
        {
            return existingUser.Id;
        }

        var user = _mapper.Map<UserEntity>(request);

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
