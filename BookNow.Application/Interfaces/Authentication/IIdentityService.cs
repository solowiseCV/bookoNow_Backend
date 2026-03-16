using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;

namespace BookNow.Application.Interfaces.Authentication;

public interface IIdentityService
{
    Task<AuthResultDto> RegisterAsync(RegisterUserRequestDto request);
    Task<AuthResultDto> LoginAsync(LoginRequestDto request);
    Task<AuthResultDto> LoginWithGoogleAsync(GoogleAuthRequestDto request, CancellationToken ct = default);
    Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<AuthResultDto> ChangePasswordAsync(ChangePasswordRequestDto request);
    Task<AuthResultDto> GetProfileAsync(string userId, CancellationToken ct = default);
    Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileRequestDto request, CancellationToken ct = default);
    Task<AuthResultDto> SendPhoneVerificationAsync(string userId, SendPhoneVerificationRequestDto request);
    Task<AuthResultDto> VerifyPhoneAsync(string userId, VerifyPhoneRequestDto request);
    Task<AuthResultDto> LogoutAsync();
}
