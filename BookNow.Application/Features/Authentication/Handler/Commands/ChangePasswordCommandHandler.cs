using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class ChangePasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ChangePasswordCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await identityService.ChangePasswordAsync(request.ChangePasswordRequest);
    }
}
