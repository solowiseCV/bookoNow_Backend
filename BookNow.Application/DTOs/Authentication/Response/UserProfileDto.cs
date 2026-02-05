namespace BookNow.Application.DTOs.Authentication.Response;

public record UserProfileDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    string Role,
    DateTime CreatedAt
);
