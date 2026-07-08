using FluentValidation;

namespace WorkBoard.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandValidator 
    : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Comment text is required.");
    }
}
