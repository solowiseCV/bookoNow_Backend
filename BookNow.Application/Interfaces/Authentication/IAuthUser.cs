using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;

namespace BookNow.Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<RegisterUserResponseDto> RegisterAsync(RegisterUserRequestDto request, CancellationToken ct);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct);
        Task<GoogleAuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request, CancellationToken ct);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ct);


    }

}
