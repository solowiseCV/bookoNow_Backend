using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Guid SenderProfileId { get; private set; }

    public string Content { get; private set; } = default!;
    public MessageSenderType SenderType { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public Conversation Conversation { get; private set; } = default!;

    protected Message() { }

    public Message(
        Guid conversationId,
        Guid senderProfileId,
        MessageSenderType senderType,
        string content)
    {
        if (conversationId == Guid.Empty)
            throw new ArgumentException("Conversation id is required.", nameof(conversationId));
        if (senderProfileId == Guid.Empty)
            throw new ArgumentException("Sender profile id is required.", nameof(senderProfileId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.", nameof(content));

        ConversationId = conversationId;
        SenderProfileId = senderProfileId;
        SenderType = senderType;
        Content = content;
        IsRead = false;
        ReadAt = null;
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
        SetUpdated();
    }
}
