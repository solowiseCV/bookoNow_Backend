namespace BookNow.Application.DTOs.Authentication.Response;

public record RegisterUserResponseDto(
    Guid UserProfileId,
    string Email,
    string Role
);
