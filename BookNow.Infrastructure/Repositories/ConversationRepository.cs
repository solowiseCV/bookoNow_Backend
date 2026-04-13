using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class ConversationRepository(BookNowDbContext context)
    : GenericRepository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> GetByIdWithParticipantsAsync(Guid id, CancellationToken ct)
        => await _dbSet
            .Include(c => c.Participants)
                .ThenInclude(p => p.Profile)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<bool> IsParticipantAsync(Guid conversationId, Guid profileId, CancellationToken ct)
        => await _context.Set<ConversationParticipant>()
            .AnyAsync(p => p.ConversationId == conversationId && p.ProfileId == profileId, ct);

    public async Task<IReadOnlyList<Conversation>> GetByParticipantAsync(Guid profileId, int pageNumber, int pageSize, CancellationToken ct)
        => await _dbSet
            .Include(c => c.Participants)
                .ThenInclude(p => p.Profile)
                    .ThenInclude(profile => profile.Workshops)
            .Include(c => c.Messages)
            .Where(c => c.Participants.Any(p => p.ProfileId == profileId))
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<int> CountByParticipantAsync(Guid profileId, CancellationToken ct)
        => await _dbSet
            .Where(c => c.Participants.Any(p => p.ProfileId == profileId))
            .AsNoTracking()
            .CountAsync(ct);

    public async Task<Conversation?> GetOneOnOneConversationAsync(Guid firstProfileId, Guid secondProfileId, CancellationToken ct)
        => await _dbSet
            .Include(c => c.Participants)
                .ThenInclude(p => p.Profile)
                    .ThenInclude(profile => profile.Workshops)
            .Include(c => c.Messages)
            .Where(c => c.Participants.Any(p => p.ProfileId == firstProfileId))
            .Where(c => c.Participants.Any(p => p.ProfileId == secondProfileId))
            .Where(c => c.Participants.Count == 2)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .FirstOrDefaultAsync(ct);
}
