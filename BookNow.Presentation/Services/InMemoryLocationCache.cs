using BookNow.Presentation.Hubs;
using Microsoft.Extensions.Caching.Memory;

namespace BookNow.Presentation.Services;


public sealed class InMemoryLocationCache(IMemoryCache cache) : ILocationCache
{

    // TTL: keep location data for 2 hours after the last update.
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(2);

    public Task SetAsync(string appointmentId, LocationPayload payload)
    {
        cache.Set(CacheKey(appointmentId), payload, Ttl);
        return Task.CompletedTask;
    }
    public Task<LocationPayload?> GetAsync(string appointmentId)
    {
        cache.TryGetValue(CacheKey(appointmentId), out LocationPayload? payload);
        return Task.FromResult(payload);
    }

    private static string CacheKey(string appointmentId) => $"mechanic-location:{appointmentId}";
}
