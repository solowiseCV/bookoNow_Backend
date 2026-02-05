using BookNow.Presentation.Middleware;
using Microsoft.AspNetCore.HttpOverrides;

namespace BookNow.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalMiddlewares(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
        });

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseCors(CorsExtensions.GetPolicyName());

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
