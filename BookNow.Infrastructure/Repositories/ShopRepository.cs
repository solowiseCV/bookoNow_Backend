using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly BookNowDbContext _context;

        public ShopRepository(BookNowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Shop shop, CancellationToken ct)
        {
            await _context.Shops.AddAsync(shop, ct);
        }

        public async Task<IEnumerable<Shop>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Shops.ToListAsync(ct);
        }

        public async Task<Shop?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Shops
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<Shop?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct)
        {
            return await _context.Shops
                .FirstOrDefaultAsync(s => s.OwnerId == ownerId, ct);
        }

        public void Update(Shop shop)
        {
            _context.Shops.Update(shop);
        }
    }
}
