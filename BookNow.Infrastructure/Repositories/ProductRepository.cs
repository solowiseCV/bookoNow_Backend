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

        public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(
            int pageNumber,
            int pageSize,
            string? search,
             string? brand,
            string? model,
            decimal? minPrice,
            decimal? maxPrice,
            Guid? shopId,
            CancellationToken ct)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            if (!string.IsNullOrEmpty(model))
            {
                query = query.Where(p => p.Model.Contains(model));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (shopId.HasValue)
            {
                query = query.Where(p => p.ShopId == shopId.Value);
            }

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
