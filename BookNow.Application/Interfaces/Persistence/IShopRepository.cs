using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IShopRepository
{
    Task<Shop?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Shop?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct);
    Task AddAsync(Shop shop, CancellationToken ct);
    void Update(Shop shop);
    Task<IEnumerable<Shop>> GetAllAsync(CancellationToken ct);
}
