namespace BookNow.Application.DTOs.Product;

public class AddProductRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrls { get; set; }
}

public class UpdateStockRequestDto
{
    public int Quantity { get; set; } // Can be positive or negative
}

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrls { get; set; }
    public Guid ShopId { get; set; }
}
