using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    void Update(Product product);
    void Delete(Product product);
}
