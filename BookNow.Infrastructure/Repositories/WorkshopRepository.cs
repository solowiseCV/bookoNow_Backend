using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public class WorkshopRepository(BookNowDbContext context)
    : GenericRepository<Workshop>(context), IWorkshopRepository
{
    public async Task<IReadOnlyList<Workshop>> GetByMechanicAsync(Guid mechanicProfileId, CancellationToken ct)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(w => w.MechanicProfileId == mechanicProfileId)
            .ToListAsync(ct);
    }

    public override async Task<Workshop?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(w => w.Reviews)
            .Include(w => w.GalleryImages)
            .FirstOrDefaultAsync(w => w.Id == id, ct);
    }

    public async Task<bool> IsOwnedByMechanicAsync(Guid workshopId, Guid mechanicProfileId, CancellationToken ct)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(w => w.Id == workshopId && w.MechanicProfileId == mechanicProfileId, ct);
    }

    public async Task<(IReadOnlyList<Workshop> Items, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct,
        double? minRating = null,
        string? search = null,
        WorkshopType? type = null)
    {
        IQueryable<Workshop> query = _dbSet.AsNoTracking();

        if (minRating.HasValue)
        {
            query = query.Where(w =>
                w.Reviews.Any() &&
                w.Reviews.Average(r => r.Rating) >= minRating.Value);
        }

        if (type.HasValue)
        {
            query = query.Where(w => w.Type == type.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.ToLower();

            query = query.Where(w =>
                w.Name.ToLower().Contains(normalizedSearch) ||
                w.Address.ToLower().Contains(normalizedSearch) ||
                w.Description.ToLower().Contains(normalizedSearch));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Include(w => w.Reviews)
            .Include(w => w.GalleryImages)
            .OrderByDescending(w => w.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}