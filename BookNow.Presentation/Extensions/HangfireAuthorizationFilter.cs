using Hangfire.Dashboard;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookNow.Infrastructure.Identity;

namespace BookNow.Presentation.Extensions;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Try to get token from query string
        var token = httpContext.Request.Query["token"].ToString();

        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            var jwtSettings = httpContext.RequestServices
                .GetRequiredService<IOptions<JwtSettings>>().Value;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)
                ),
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            //  Check for Admin role
            return principal.HasClaim(c => 
                (c.Type == ClaimTypes.Role || c.Type == "role") && 
                c.Value == "Admin");
        }
        catch
        {
            // Token validation failed
            return false;
        }
    }
}
