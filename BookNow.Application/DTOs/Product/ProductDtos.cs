using BookNow.Domain.Enums;

namespace BookNow.Application.DTOs.Product;

public class AddProductRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Model { get; set; } =default!;
    public int Year { get; set; }
    public string Brand { get; set; } = default!;
    public string PartNumber { get; set; } = default!;
}

public class UpdateStockRequestDto
{
    public int Quantity { get; set; }
}

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public string PartNumber { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public Guid ShopId { get; set; }
}
