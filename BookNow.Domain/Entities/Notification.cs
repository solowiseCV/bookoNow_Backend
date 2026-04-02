namespace BookNow.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }

    protected Notification() { }

    public Notification(Guid userId, string message)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.", nameof(message));

        UserId = userId;
        Message = message;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        if (IsRead)
            return;

        IsRead = true;
        SetUpdated();
    }
}
