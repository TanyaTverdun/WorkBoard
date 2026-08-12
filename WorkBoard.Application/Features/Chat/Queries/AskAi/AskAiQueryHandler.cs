using MediatR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.Chat.Queries.AskAi;

public class AskAiQueryHandler : IRequestHandler<AskAiQuery, string>
{
    private readonly Kernel _kernel;
    private readonly IUserContext _userContext;

    public AskAiQueryHandler(
        Kernel kernel,
        IUserContext userContext)
    {
        _kernel = kernel;
        _userContext = userContext;
    }

    public async Task<string> Handle(
        AskAiQuery request,
        CancellationToken cancellationToken)
    {
        await _userContext.SetWorkspaceContextAsync(
            request.WorkspaceId,
            cancellationToken);

        var chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();

        var userRole = _userContext.CurrentWorkspaceRole?.ToString() ?? "Guest";

        var systemPrompt = $"""
            You are a professional and helpful AI assistant 
            for the WorkBoard application.
            You are currently assisting a user within 
            a specific Workspace context. 
            The user's role in this current Workspace is: '{userRole}'.

            CRITICAL INSTRUCTIONS:
            1. ROLE-BASED ACCESS & SENSITIVE DATA:
               - The user has the '{userRole}' role.
               - Only "Owner" roles have access to sensitive configurations 
               such as API integrations (Slack secrets), default billing 
               mailboxes, and enterprise promotional codes.
               - The Knowledge Base tool automatically filters data based on 
               this role. If the tool does not return sensitive data, 
               do NOT guess or hallucinate it.
               - If a non-Owner user asks for sensitive admin information, 
               politely explain that this information is restricted 
               to Workspace Owners only.

            2. MANDATORY TOOL USAGE:
               - You MUST use the Knowledge Base (SearchKnowledgeBaseAsync) 
               tool to answer ANY questions about WorkBoard functionality.
               - Key documentation topics include: Workspace and Board management, 
               Kanban board usage (members, filters, task cards, labels, 
               checklists, due dates, attachments), Archivation Status Tracker, 
               and Profile settings.

            3. NO HALLUCINATIONS:
               - Base your answers STRICTLY on the context provided 
               by the Knowledge Base tool.
               - If the tool returns "No relevant information...", explicitly 
               state that you do not have that information in the documentation. 
               Do not invent WorkBoard features or UI elements.

            4. TONE & LANGUAGE:
               - Always respond in the exact language the user uses to address you.
               - Provide clear, step-by-step instructions when asked "how to" do something.
            """;

        var chatHistory = new ChatHistory(systemPrompt);

        foreach (var msg in request.Messages)
        {
            if (msg.Role.Equals(AIChatConstants.Roles.User, 
                StringComparison.OrdinalIgnoreCase))
            {
                chatHistory.AddUserMessage(msg.Content);
            }
            else if (msg.Role.Equals(AIChatConstants.Roles.Assistant, 
                StringComparison.OrdinalIgnoreCase))
            {
                chatHistory.AddAssistantMessage(msg.Content);
            }
        }

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        var response = await chatCompletion.GetChatMessageContentAsync(
            chatHistory,
            executionSettings: executionSettings,
            kernel: _kernel,
            cancellationToken: cancellationToken);

        return response.Content ?? AIChatConstants.ErrorMessages.GenerationFailed;
    }
}
