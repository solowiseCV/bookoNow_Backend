using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookNow.Infrastructure.Repositories;

public sealed class WorkshopReadRepository(BookNowDbContext context) : IWorkshopReadRepository
{
    public async Task<IReadOnlyList<Workshop>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken ct)
    {
        double latDelta = radiusKm / 111.0;
        double lonDelta = radiusKm / (111.0 * Math.Cos(ToRadians(latitude)));
        double minLat = latitude - latDelta;
        double maxLat = latitude + latDelta;
        double minLon = longitude - lonDelta;
        double maxLon = longitude + lonDelta;

        var candidates = await context.Workshops
            .AsNoTracking()
            .Include(w => w.Reviews)
            .Where(w =>
                w.Latitude >= minLat && w.Latitude <= maxLat &&
                w.Longitude >= minLon && w.Longitude <= maxLon)
            .ToListAsync(ct);

        var nearby = candidates
            .Select(w => new
            {
                Workshop = w,
                Distance = HaversineKm(latitude, longitude, w.Latitude, w.Longitude)
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Select(x => x.Workshop)
            .ToList();

        return nearby;
    }

    private static double ToRadians(double angle) => angle * (Math.PI / 180);

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a =
            Math.Pow(Math.Sin(dLat / 2), 2) +
            Math.Cos(ToRadians(lat1)) *
            Math.Cos(ToRadians(lat2)) *
            Math.Pow(Math.Sin(dLon / 2), 2);

        double c = 2 * Math.Asin(Math.Sqrt(a));

        return R * c;
    }
}
