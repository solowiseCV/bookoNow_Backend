

using BookNow.Domain.Enums;

namespace BookNow.Application.DTOs.Workshop;

public sealed record WorkshopDto(
    Guid Id,
    string Name,
    string Description,
    string Address,
    double Latitude,
    double Longitude,
    bool IsVerified,
    WorkshopType Type,

    string? PhoneNumber,
    string? OpeningHours,
    string? HeroImageUrl,
     List<string> GalleryImages, 
     List<string> Reviews,
    double AverageRating,
    int ReviewCount,
    string? OwnerName = null,
    string? OwnerEmail = null
);
public class WorkshopSubscribeRequestDto
{
    public Guid WorkshopId { get; set; }
    public string Email { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
}
public class RegisterSubaccountRequestDto
{
    public Guid WorkshopId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
}