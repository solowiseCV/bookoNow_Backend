
namespace BookNow.Application.DTOs.Authentication.Response;

public record GoogleAuthResponseDto(
    string AccessToken,
    DateTime ExpiresAt,
    UserSummaryDto User
);
