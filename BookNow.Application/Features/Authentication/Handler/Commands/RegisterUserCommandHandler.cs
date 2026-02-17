using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RegisterAsync(request.RegisterRequest);
    }
}
