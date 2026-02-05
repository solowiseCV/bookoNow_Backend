using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;

public interface IWorkshopRepository : IGenericRepository<Workshop>
{
    Task<IReadOnlyList<Workshop>> GetByMechanicAsync(Guid mechanicProfileId, CancellationToken ct);
    Task<bool> IsOwnedByMechanicAsync(Guid workshopId, Guid mechanicProfileId, CancellationToken ct);

}
