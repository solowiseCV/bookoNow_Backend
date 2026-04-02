using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class GoogleAuthCommandHandler(IIdentityService identityService) : IRequestHandler<GoogleAuthCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        return await identityService.LoginWithGoogleAsync(request.GoogleAuthRequest, cancellationToken);
    }
}
