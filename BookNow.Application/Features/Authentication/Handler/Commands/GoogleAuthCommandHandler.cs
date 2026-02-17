using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;

    public GoogleAuthCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginWithGoogleAsync(request.GoogleAuthRequest);
    }
}
