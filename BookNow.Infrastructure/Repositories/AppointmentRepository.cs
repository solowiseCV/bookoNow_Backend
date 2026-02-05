using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class AppointmentRepository
    : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(BookNowDbContext context)
        : base(context) { }

    public async Task<IReadOnlyList<Appointment>> GetByClientAsync(Guid clientProfileId, CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.ClientProfileId == clientProfileId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Appointment>> GetByWorkshopAsync(
        Guid workshopId,
        CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.WorkshopId == workshopId)
            .ToListAsync(ct);
    }
}
