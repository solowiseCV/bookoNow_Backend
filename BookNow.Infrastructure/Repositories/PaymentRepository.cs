using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories
{
    public class PaymentRepository(BookNowDbContext context) : IPaymentRepository
    {
        public async Task AddAsync(Payment payment, CancellationToken ct)
        {
            await context.Payments.AddAsync(payment, ct);
        }

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await context.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task<Payment?> GetByReferenceAsync(string reference, CancellationToken ct)
        {
            return await context.Payments.FirstOrDefaultAsync(p => p.Reference == reference, ct);
        }

        public void Update(Payment payment)
        {
            context.Payments.Update(payment);
        }
    }
}
