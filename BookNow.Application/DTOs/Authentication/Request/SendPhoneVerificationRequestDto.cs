namespace BookNow.Application.DTOs.Authentication.Request;


public record SendPhoneVerificationRequestDto(string PhoneNumber);
public record VerifyPhoneRequestDto(string Code);