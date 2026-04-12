using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByIdWithParticipantsAsync(Guid id, CancellationToken ct);
    Task<bool> IsParticipantAsync(Guid conversationId, Guid profileId, CancellationToken ct);
    Task<IReadOnlyList<Conversation>> GetByParticipantAsync(Guid profileId, int pageNumber, int pageSize, CancellationToken ct);
    Task<int> CountByParticipantAsync(Guid profileId, CancellationToken ct);
    Task<Conversation?> GetOneOnOneConversationAsync(Guid firstProfileId, Guid secondProfileId, CancellationToken ct);
}
