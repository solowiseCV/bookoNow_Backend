using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<(IReadOnlyList<Message> Messages, string? NextCursor)> GetMessagesAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken ct);
    Task<IReadOnlyList<Message>> GetUnreadMessagesForConversationAsync(Guid conversationId, Guid recipientProfileId, CancellationToken ct);
}
