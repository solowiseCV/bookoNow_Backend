namespace BookNow.Application.DTOs.Shop;

public class CreateShopRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? LogoUrl { get; set; }
}

public class ShopResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? LogoUrl { get; set; }
}
