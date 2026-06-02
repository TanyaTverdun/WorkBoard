using MediatR;

namespace WorkBoard.Application.Features.User.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }

    public string? FullName { get; set; }

    public required string Email { get; set; }
}
