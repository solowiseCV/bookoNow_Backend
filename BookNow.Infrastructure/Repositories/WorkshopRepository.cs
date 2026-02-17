using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;
public class WorkshopRepository(BookNowDbContext context)
    : GenericRepository<Workshop>(context), IWorkshopRepository
{
    public async Task<IReadOnlyList<Workshop>> GetByMechanicAsync(Guid mechanicProfileId, CancellationToken ct)
    {
        return await _dbSet.Where(w => w.MechanicProfileId == mechanicProfileId).ToListAsync(ct);
    }

    public async Task<bool> IsOwnedByMechanicAsync(Guid workshopId, Guid mechanicProfileId, CancellationToken ct)
    {
        return await _dbSet.AnyAsync(w => w.Id == workshopId && w.MechanicProfileId == mechanicProfileId, ct);
    }
}
