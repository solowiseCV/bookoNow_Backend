namespace BookNow.Application.DTOs.Shop;

public class CreateShopRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class ShopResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string Status { get; set; } = null!;
    public bool IsSubscribed { get; set; }
    public DateTime? VerifiedAt { get; set; }
}

public class SubscribeRequestDto
{
    public Guid ShopId { get; set; }
    public string Email { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
}