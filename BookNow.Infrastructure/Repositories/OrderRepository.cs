using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class OrderRepository(BookNowDbContext context) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken ct)
        {
            await context.Orders.AddAsync(order, ct);
        }

        public async Task<IEnumerable<Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken ct)
        {
            return await context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId== buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<Order?> GetWithItemsByIdAsync(Guid id, CancellationToken ct)
        {
            return await context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public void Update(Order order)
        {
            context.Orders.Update(order);
        }
    }
}
