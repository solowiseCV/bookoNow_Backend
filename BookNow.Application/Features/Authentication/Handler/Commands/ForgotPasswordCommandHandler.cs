using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class ForgotPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ForgotPasswordCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        return await identityService.ForgotPasswordAsync(request.ForgotPasswordRequest);
    }
}
