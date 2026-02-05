
namespace BookNow.Application.DTOs.Authentication.Response;
public record LoginResponseDto(
    string AccessToken,
    DateTime ExpiresAt,
    UserSummaryDto User
);
