using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IShopRepository : IGenericRepository<Shop>
{
    Task<Shop?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct);
    Task<(IEnumerable<Shop> Items, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct, BookNow.Domain.Enums.ShopStatus? status = null, bool? isVerified = null);
}
