namespace WorkBoard.Application.Common.Dtos.Chat;

public class ChatRequestDto
{
    public required List<ChatMessageDto> Messages { get; set; }
}
