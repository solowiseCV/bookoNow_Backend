namespace BookNow.Application.DTOs.Authentication.Response;

public record AuthResultDto(
    bool Success,
    string Message,
    IEnumerable<string>? Errors,
    string? Token = null
);
