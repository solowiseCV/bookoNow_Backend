namespace BookNow.Domain.Entities;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Guid ProfileId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public Conversation Conversation { get; private set; } = default!;
    public UserProfile Profile { get; private set; } = default!;

    protected ConversationParticipant() { }

    public ConversationParticipant(Guid conversationId, Guid profileId)
    {
        if (conversationId == Guid.Empty)
            throw new ArgumentException("Conversation id is required.", nameof(conversationId));
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile id is required.", nameof(profileId));

        ConversationId = conversationId;
        ProfileId = profileId;
        JoinedAt = DateTime.UtcNow;
    }
}
