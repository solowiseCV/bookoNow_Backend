using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly BookNowDbContext _context;

        public PaymentRepository(BookNowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment, CancellationToken ct)
        {
            await _context.Payments.AddAsync(payment, ct);
        }

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken ct)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Reference == reference, ct);
        }

        public void Update(Payment payment)
        {
            _context.Payments.Update(payment);
        }
    }
}
