using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;

    public LogoutUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LogoutAsync();
    }
}
