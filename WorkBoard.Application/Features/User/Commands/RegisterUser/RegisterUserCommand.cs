using MediatR;

namespace WorkBoard.Application.Features.User.Commands.RegisterUser;

/// <summary>
/// Command to register a user authenticated
/// </summary>
public class RegisterUserCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets or sets the unique Object ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user
    /// </summary>
    public required string Email { get; set; }
}
