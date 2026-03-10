namespace BookNow.Application.DTOs.Authentication.Request;

public record UpdateProfileRequestDto(
    string FullName,
    string? PhoneNumber
);
