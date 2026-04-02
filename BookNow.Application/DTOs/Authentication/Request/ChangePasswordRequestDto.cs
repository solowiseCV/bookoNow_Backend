namespace BookNow.Application.DTOs.Authentication.Request;

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword
);
