namespace BookNow.Application.DTOs.Authentication.Response;

public record RefreshTokenResponseDto(
    string AccessToken,
    DateTime ExpiresAt
);
