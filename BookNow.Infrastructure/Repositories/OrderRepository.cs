using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BookNowDbContext _context;

        public OrderRepository(BookNowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order, CancellationToken ct)
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public async Task<IEnumerable<Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken ct)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.BuyerId == buyerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task<Order?> GetWithItemsByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
    }
}
