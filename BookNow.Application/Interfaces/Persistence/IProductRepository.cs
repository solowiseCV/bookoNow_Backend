using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId, CancellationToken ct);
    Task<int> GetCountByShopIdAsync(Guid shopId, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    void Update(Product product);
    void Delete(Product product);
    Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(
        int pageNumber, 
        int pageSize, 
        string? search, 
        string brand, 
        string? model, 
        decimal? minPrice, 
        decimal? maxPrice, 
        Guid? shopId, 
        CancellationToken ct);
}
