using BookNow.Application.Models;

namespace BookNow.Application.Interfaces.Services;

public interface IGeocodingService
{
    Task<GeoCoordinates> GeocodeAsync(string address, CancellationToken cancellationToken);
}
