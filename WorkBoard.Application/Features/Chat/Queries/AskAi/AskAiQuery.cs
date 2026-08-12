using MediatR;
using WorkBoard.Application.Common.Dtos.Chat;

namespace WorkBoard.Application.Features.Chat.Queries.AskAi;

public record AskAiQuery(
    Guid WorkspaceId,
    List<ChatMessageDto> Messages) : IRequest<string>;
