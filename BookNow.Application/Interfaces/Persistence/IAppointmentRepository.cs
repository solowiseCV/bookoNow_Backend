using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByClientAsync(Guid clientProfileId, CancellationToken ct);
    Task<IReadOnlyList<Appointment>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct);
    Task<Appointment?> GetWithAttachmentsByIdAsync(Guid id, CancellationToken ct);
}
