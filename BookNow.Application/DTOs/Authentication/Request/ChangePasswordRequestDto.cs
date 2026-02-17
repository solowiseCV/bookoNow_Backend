namespace BookNow.Application.DTOs.Authentication.Request;

public record ChangePasswordRequestDto(
    string UserId,
    string CurrentPassword,
    string NewPassword
);
