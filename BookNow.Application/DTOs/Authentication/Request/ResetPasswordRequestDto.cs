namespace BookNow.Application.DTOs.Authentication.Request;

public record ResetPasswordRequestDto(
    string Email,
    string Token,
    string NewPassword
);
