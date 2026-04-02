using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class LogoutUserCommandHandler(IIdentityService identityService) : IRequestHandler<LogoutUserCommand, AuthResultDto>
{
   public async Task<AuthResultDto> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
{
    return await identityService.LogoutAsync(request.UserId, request.Token);
}};

