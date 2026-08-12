using FluentValidation;

namespace WorkBoard.Application.Features.Chat.Queries.AskAi;

public class AskAiQueryValidator : AbstractValidator<AskAiQuery>
{
    public AskAiQueryValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
            .WithMessage("Workspace ID is required and cannot be empty.");

        RuleFor(x => x.Messages)
            .NotEmpty()
            .WithMessage("Chat history cannot be empty.");
    }
}
