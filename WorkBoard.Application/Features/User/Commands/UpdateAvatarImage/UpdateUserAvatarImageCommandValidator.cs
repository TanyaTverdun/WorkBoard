using FluentValidation;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarImage
{
    public class UpdateUserAvatarImageCommandValidator 
        : AbstractValidator<UpdateUserAvatarImageCommand>
    {
        public UpdateUserAvatarImageCommandValidator()
        {
            RuleFor(x => x.Length)
                .GreaterThan(0)
                .WithMessage("File cannot be empty.")
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("File size must not exceed 5 MB.");

            RuleFor(x => x.ContentType)
                .Must(contentType => contentType.StartsWith("image/"))
                .WithMessage("Only image files are allowed.");
        }
    }
}
