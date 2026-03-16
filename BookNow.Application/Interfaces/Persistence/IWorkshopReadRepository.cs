using BookNow.Domain.Entities;

namespace BookNow.Application.Interfaces.Persistence;
public interface IWorkshopReadRepository
{
    Task<IReadOnlyList<Workshop>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken ct);
}
