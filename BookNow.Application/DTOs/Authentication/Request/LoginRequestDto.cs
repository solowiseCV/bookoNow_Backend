namespace BookNow.Application.DTOs.Authentication.Request;

public record LoginRequestDto(
    string Email,
    string Password
);
