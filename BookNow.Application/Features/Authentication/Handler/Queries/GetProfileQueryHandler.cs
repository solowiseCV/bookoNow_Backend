using BookNow.Application.DTOs.Authentication.Response;
using BookNow.Application.Features.Authentication.Request.Queries;
using BookNow.Application.Interfaces.Authentication;
using MediatR;

namespace BookNow.Application.Features.Authentication.Handler.Queries;

public class GetProfileQueryHandler(IIdentityService identityService) : IRequestHandler<GetProfileQuery, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        return await identityService.GetProfileAsync(request.UserId, cancellationToken);
    }
}
