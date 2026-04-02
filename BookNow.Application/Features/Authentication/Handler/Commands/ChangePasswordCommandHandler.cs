using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class ChangePasswordCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangePasswordCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<AuthResultDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Get the logged-in user ID from JWT claims
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new AuthResultDto(false, "User not authenticated", new[] { "No user ID found in token." });
        }
        return await _identityService.ChangePasswordAsync(userId, request.ChangePasswordRequest);
    }
}