using BookNow.Presentation.Hubs;
using Microsoft.Extensions.Caching.Memory;

namespace BookNow.Presentation.Services;

/// <summary>
/// In-process memory cache for last-known mechanic location.
/// Swap for a distributed cache (Redis) in multi-instance production deployments.
/// </summary>
public sealed class InMemoryLocationCache : ILocationCache
{
    private readonly IMemoryCache _cache;

    // TTL: keep location data for 2 hours after the last update.
    private static readonly TimeSpan Ttl = TimeSpan.FromHours(2);

    public InMemoryLocationCache(IMemoryCache cache) => _cache = cache;

    public Task SetAsync(string appointmentId, LocationPayload payload)
    {
        _cache.Set(CacheKey(appointmentId), payload, Ttl);
        return Task.CompletedTask;
    }

    public Task<LocationPayload?> GetAsync(string appointmentId)
    {
        _cache.TryGetValue(CacheKey(appointmentId), out LocationPayload? payload);
        return Task.FromResult(payload);
    }

    private static string CacheKey(string appointmentId) => $"mechanic-location:{appointmentId}";
}
