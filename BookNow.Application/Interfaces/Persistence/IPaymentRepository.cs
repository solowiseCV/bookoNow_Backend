using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Payment?> GetByReferenceAsync(string reference, CancellationToken ct);
    Task AddAsync(Payment payment, CancellationToken ct);
    void Update(Payment payment);
}
