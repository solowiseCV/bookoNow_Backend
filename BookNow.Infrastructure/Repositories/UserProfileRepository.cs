using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class UserProfileRepository
    : GenericRepository<UserProfile>, IUserProfileRepository
{
    public UserProfileRepository(BookNowDbContext context) : base(context) { }

    public override async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _dbSet
            .Include(x => x.Workshops)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<UserProfile?> GetByIdentityIdAsync(Guid identityUserId, CancellationToken ct)
    {
        return await _dbSet
            .Include(x => x.Workshops)
            .FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId, ct);
    }
}
