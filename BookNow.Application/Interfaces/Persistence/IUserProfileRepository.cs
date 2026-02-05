using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IUserProfileRepository
{
    Task AddAsync(UserProfile profile, CancellationToken ct);
    Task<UserProfile?> GetByIdentityIdAsync(Guid identityUserId, CancellationToken ct);
}
