using BookNow.Application.Common.Options;
using BookNow.Application.Interfaces.Services;
using BookNow.Application.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookNow.Infrastructure.ExternalServices.Geocoding;

public sealed class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly GeocodingOptions _options;

    public NominatimGeocodingService(HttpClient httpClient, IOptions<GeocodingOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        }

        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("BookNow", "1.0"));
    }

    public async Task<GeoCoordinates> GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required for geocoding.", nameof(address));

        var url = $"/search?format=json&q={Uri.EscapeDataString(address)}&limit=1&addressdetails=0";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Unable to resolve the address to coordinates at this time.");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var results = JsonSerializer.Deserialize<List<NominatimResponse>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var place = results?.FirstOrDefault();
        if (place is null || string.IsNullOrWhiteSpace(place.Lat) || string.IsNullOrWhiteSpace(place.Lon))
        {
            throw new InvalidOperationException("Unable to derive coordinates from the provided address.");
        }

        return new GeoCoordinates(double.Parse(place.Lat, System.Globalization.CultureInfo.InvariantCulture),
            double.Parse(place.Lon, System.Globalization.CultureInfo.InvariantCulture));
    }

    private sealed record NominatimResponse(string Lat, string Lon);
}
