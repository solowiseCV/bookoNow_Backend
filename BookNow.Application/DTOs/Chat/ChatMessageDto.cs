using BookNow.Domain.Enums;

namespace BookNow.Application.DTOs.Chat;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderProfileId { get; set; }
    public MessageSenderType SenderType { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
