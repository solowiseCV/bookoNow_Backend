namespace BookNow.Application.DTOs.Authentication.Response;

public record UserSummaryDto(
    Guid UserProfileId,
    string FullName,
    string Email,
    string Role
);
