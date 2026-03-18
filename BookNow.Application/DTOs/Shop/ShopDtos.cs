using BookNow.Application.DTOs.Product;

namespace BookNow.Application.DTOs.Shop;

public class CreateShopRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? OpeningHours { get; set; }
}

public class ShopResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string PhoneNumber { get; set; } =string.Empty;
    public string OpeningHours { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Status { get; set; } = null!;
    public bool IsSubscribed { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public List<ProductResponseDto> Products { get; set; } = new();
}

public class SubscribeRequestDto
{
    public Guid ShopId { get; set; }
    public string Email { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
}