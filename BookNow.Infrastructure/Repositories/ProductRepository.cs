using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly BookNowDbContext _context;

        public ProductRepository(BookNowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            await _context.Products.AddAsync(product, ct);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId, CancellationToken ct)
        {
            return await _context.Products
                .Where(p => p.ShopId == shopId)
                .ToListAsync(ct);
        }

        public async Task<int> GetCountByShopIdAsync(Guid shopId, CancellationToken ct)
        {
            return await _context.Products.CountAsync(p => p.ShopId == shopId, ct);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }
    }
}
