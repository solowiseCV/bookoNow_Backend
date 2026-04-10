using BookNow.Application.Common.Options;
using BookNow.Application.Interfaces.Services;
using BookNow.Infrastructure.ExternalServices.Geocoding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

namespace BookNow.Infrastructure.Extensions;

public static class GeocodingServiceExtensions
{
    public static IServiceCollection AddGeocodingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GeocodingOptions>(configuration.GetSection(GeocodingOptions.SectionName));

        services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection(GeocodingOptions.SectionName)[nameof(GeocodingOptions.BaseUrl)] ?? "https://nominatim.openstreetmap.org");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BookNow/1.0");
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(20));
    }
}
