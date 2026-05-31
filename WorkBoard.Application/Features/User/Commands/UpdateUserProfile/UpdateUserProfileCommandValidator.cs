using FluentValidation;

namespace WorkBoard.Application.Features.User.Commands.UpdateUserProfile;

/// <summary>
/// Provides validation rules for the 
/// <see cref="UpdateUserProfileCommand"/>.
/// </summary>
public class UpdateUserProfileCommandValidator 
    : AbstractValidator<UpdateUserProfileCommand>
{
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UpdateUserProfileCommandValidator"/> class and defines rules
    /// </summary>
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(v => v.FullName)
            .MaximumLength(100)
            .WithMessage("Full Name must not exceed 100 characters.");

        RuleFor(v => v.AvatarUrl)
            .MaximumLength(2048)
            .WithMessage("Avatar URL must not exceed 2048 characters.");
    }
}
