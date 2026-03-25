using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookNow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace BookNow.Presentation.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Prevent automatic claim mapping
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Set to true in production
                    ValidateAudience = false, // Set to true in production
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)
                    ),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
{
    OnTokenValidated = async context =>
    {
        var dbContext = context.HttpContext.RequestServices
            .GetRequiredService<BookNowDbContext>();

        var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);

        if (!string.IsNullOrEmpty(jti))
        {
            var isRevoked = await dbContext.RevokedTokens
                .AnyAsync(t => t.Jti == jti);

            if (isRevoked)
                context.Fail("Token has been revoked");
        }
    }
};
            });
            
        return services;
    }
}