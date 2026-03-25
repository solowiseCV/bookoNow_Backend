using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Order?> GetWithItemsByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken ct);
    Task<IEnumerable<Order>> GetOrdersByShopOwnerIdAsync(Guid ownerId, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    void Update(Order order);
}
