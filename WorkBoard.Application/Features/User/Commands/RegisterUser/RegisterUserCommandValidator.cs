using FluentValidation;

namespace WorkBoard.Application.Features.User.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty()
                .WithMessage("User ID is required");

        RuleFor(v => v.Email)
            .NotEmpty()
                .WithMessage("Email is required")
            .EmailAddress()
                .WithMessage("Invalid email format")
            .MaximumLength(150)
                .WithMessage("Email must not exceed 150 characters");

        RuleFor(v => v.FullName)
            .MaximumLength(100)
                .WithMessage("Full Name must not exceed 100 characters");
    }
}
