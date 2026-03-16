using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class AppointmentRepository(BookNowDbContext context) : GenericRepository<Appointment>(context), IAppointmentRepository

{
    public async Task<IReadOnlyList<Appointment>> GetByClientAsync(Guid clientProfileId, CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.ClientProfileId == clientProfileId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Appointment>> GetByWorkshopAsync(Guid workshopId, CancellationToken ct)
    {
        return await _dbSet
            .Where(a => a.WorkshopId == workshopId)
            .ToListAsync(ct);
    }

    public async Task<Appointment?> GetWithAttachmentsByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }
}
