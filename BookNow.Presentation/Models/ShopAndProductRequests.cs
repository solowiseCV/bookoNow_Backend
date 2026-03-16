using Microsoft.AspNetCore.Http;

namespace BookNow.Presentation.Models;

public class CreateShopRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IFormFile? Logo { get; set; }
}

public class AddProductRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public List<IFormFile>? Images { get; set; }
}
