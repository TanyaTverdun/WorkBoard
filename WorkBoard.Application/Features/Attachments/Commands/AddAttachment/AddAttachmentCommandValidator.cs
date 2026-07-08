using FluentValidation;

namespace WorkBoard.Application.Features.Attachments.Commands.AddAttachment;

public class AddAttachmentCommandValidator
    : AbstractValidator<AddAttachmentCommand>
{
    public AddAttachmentCommandValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required.");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .WithMessage("File cannot be empty.")
            .LessThanOrEqualTo(100 * 1024 * 1024)
            .WithMessage("File size must not exceed 100 MB.");

        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required.");
    }
}
