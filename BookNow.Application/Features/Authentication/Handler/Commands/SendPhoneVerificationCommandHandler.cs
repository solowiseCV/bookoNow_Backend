using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Request.Commands;

public class SendPhoneVerificationCommandHandler(IIdentityService _identityService) 
    : IRequestHandler<SendPhoneVerificationCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(SendPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SendPhoneVerificationAsync(request.UserId, request.Request);
    }
}
