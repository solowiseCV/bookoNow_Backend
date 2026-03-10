using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Commands;

public class UpdateProfileCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateProfileCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        return await identityService.UpdateProfileAsync(request.UserId, request.Request, cancellationToken);
    }
}
