using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId, CancellationToken ct);
    Task<int> GetCountByShopIdAsync(Guid shopId, CancellationToken ct);
    Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(
        int pageNumber, 
        int pageSize, 
        string? search, 
        string? brand, 
        string? model, 
        decimal? minPrice, 
        decimal? maxPrice, 
        Guid? shopId, 
        CancellationToken ct);
}
