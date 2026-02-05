
using BookNow.Application.Interfaces.Auth;

namespace BookNow.Application.Interfaces.Authentication
{
   
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo> ValidateTokenAsync(string idToken, CancellationToken ct);


    }

}
