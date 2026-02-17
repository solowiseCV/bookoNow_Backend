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
}
