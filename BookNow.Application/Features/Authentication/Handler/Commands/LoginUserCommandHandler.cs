using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.LoginRequest);
    }
}
