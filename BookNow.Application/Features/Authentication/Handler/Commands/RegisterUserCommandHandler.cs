using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class RegisterUserCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RegisterAsync(request.RegisterRequest);
    }
}
