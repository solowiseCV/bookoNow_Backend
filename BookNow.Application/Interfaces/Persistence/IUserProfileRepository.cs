using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IUserProfileRepository : IGenericRepository<UserProfile>
{
    Task<UserProfile?> GetByIdentityIdAsync(Guid identityUserId, CancellationToken ct);
}
