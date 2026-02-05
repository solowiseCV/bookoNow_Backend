using Microsoft.Extensions.DependencyInjection;

namespace BookNow.Api.Extensions;

public static class CorsExtensions
{
    private const string PolicyName = "AllowAll";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    public static string GetPolicyName() => PolicyName;
}
