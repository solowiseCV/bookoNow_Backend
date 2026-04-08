using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class ShopRepository(BookNowDbContext context) : GenericRepository<Shop>(context), IShopRepository
    {
        public override async Task<Shop?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _dbSet
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<Shop?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.OwnerId == ownerId, ct);
        }

        public async Task<(IEnumerable<Shop> Items, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct, BookNow.Domain.Enums.ShopStatus? status = null, bool? isVerified = null)
        {
            var query = _dbSet
                .Include(s => s.Owner)
                .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            if (isVerified.HasValue)
            {
                query = isVerified.Value ? query.Where(s => s.VerifiedAt != null) : query.Where(s => s.VerifiedAt == null);
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
