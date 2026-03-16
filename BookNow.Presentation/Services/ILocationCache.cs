using BookNow.Presentation.Hubs;

namespace BookNow.Presentation.Services;


public interface ILocationCache
{
    Task SetAsync(string appointmentId, LocationPayload payload);
    Task<LocationPayload?> GetAsync(string appointmentId);
}
