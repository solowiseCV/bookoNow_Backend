using BookNow.Domain.Enums;

namespace BookNow.Application.DTOs.Authentication.Request;

public record RegisterUserRequestDto(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    UserRole Role
);
