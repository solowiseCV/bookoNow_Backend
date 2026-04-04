using BookNow.Domain.Entities;
using BookNow.Domain.Enums;

namespace BookNow.Application.Interfaces.Persistence;

public interface IWorkshopRepository : IGenericRepository<Workshop>
{
    Task<IReadOnlyList<Workshop>> GetByMechanicAsync(Guid mechanicProfileId, CancellationToken ct);
    Task<bool> IsOwnedByMechanicAsync(Guid workshopId, Guid mechanicProfileId, CancellationToken ct);
    Task<(IReadOnlyList<Workshop> Items, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct, double? minRating = null, string? search = null, WorkshopType? type = null, bool? isVerified = null);
}
