using BookNow.Presentation.Hubs;

namespace BookNow.Presentation.Services;

/// <summary>
/// Stores the last-known mechanic location per appointment so late-joining
/// clients can receive it immediately on connection.
/// </summary>
public interface ILocationCache
{
    Task SetAsync(string appointmentId, LocationPayload payload);
    Task<LocationPayload?> GetAsync(string appointmentId);
}
