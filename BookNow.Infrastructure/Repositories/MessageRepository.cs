using System.Text;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class MessageRepository(BookNowDbContext context)
    : GenericRepository<Message>(context), IMessageRepository
{
    public async Task<(IReadOnlyList<Message> Messages, string? NextCursor)> GetMessagesAsync(Guid conversationId, string? cursor, int pageSize, CancellationToken ct)
    {
        var query = _dbSet.AsNoTracking()
            .Where(m => m.ConversationId == conversationId);

        var cursorValue = DecodeCursor(cursor);
        if (cursorValue != null)
        {
            query = query.Where(m => m.CreatedAt < cursorValue.Value.CreatedAt
                || (m.CreatedAt == cursorValue.Value.CreatedAt && m.Id < cursorValue.Value.MessageId));
        }

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(pageSize + 1)
            .ToListAsync(ct);

        string? nextCursor = null;
        if (items.Count > pageSize)
        {
            var next = items[pageSize];
            items.RemoveAt(pageSize);
            nextCursor = EncodeCursor(next.CreatedAt, next.Id);
        }

        return (items, nextCursor);
    }

    private static string EncodeCursor(DateTime createdAt, Guid messageId)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{createdAt.Ticks}:{messageId:N}"));

    private static (DateTime CreatedAt, Guid MessageId)? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = decoded.Split(':', 2);
            if (parts.Length != 2) return null;
            var ticks = long.Parse(parts[0]);
            var messageId = Guid.Parse(parts[1]);
            return (new DateTime(ticks, DateTimeKind.Utc), messageId);
        }
        catch
        {
            return null;
        }
    }
}
