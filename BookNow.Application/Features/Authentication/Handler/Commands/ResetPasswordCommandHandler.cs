using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class ResetPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ResetPasswordCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await identityService.ResetPasswordAsync(request.ResetPasswordRequest);
    }
}
