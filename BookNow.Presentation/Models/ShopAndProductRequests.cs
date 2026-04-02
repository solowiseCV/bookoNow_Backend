using BookNow.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace BookNow.Presentation.Models;

public class CreateShopRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? OpeningHours { get; set; }
    public IFormFile? Logo { get; set; }
}

public class AddProductRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Year {get; set;}
    public string PartNumber { get; set; } = default!;
    public string Model {get; set;} = string.Empty;
    public string Brand { get; set; } = default!;
    public List<IFormFile>? Images { get; set; }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int Year {get; set;}
    public string Model {get; set;} = string.Empty;
    public string PartNumber {get; set;} = default!;
    public string Brand { get; set; } = default!;
    public List<IFormFile>? Images { get; set; }
    public List<string>? ImageUrlsToKeep { get; set; }
}
