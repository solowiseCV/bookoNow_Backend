using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class VerifyPhoneCommandHandler(IIdentityService _identityService) 
    : IRequestHandler<VerifyPhoneCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.VerifyPhoneAsync(request.UserId, request.Request);
    }
}
