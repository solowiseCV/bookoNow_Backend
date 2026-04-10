namespace BookNow.Application.Common.Options;

public class GeocodingOptions
{
    public const string SectionName = "Geocoding";
    public string BaseUrl { get; set; } = "https://nominatim.openstreetmap.org";
}
