

namespace BookNow.Domain.Entities;

public class Product : BaseEntity
{
    public  string Name { get; private set; } = default!;
    public  string Description { get; private set; } = default!;
    public  decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string? ImageUrls { get; private set; }
    public Guid ShopId { get; private set; }
    public Shop Shop { get; private set; } = default!;

    private Product() { }  

    public Product(string name, string description, decimal price, int stockQuantity, Guid shopId, string? imageUrls = null)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        ShopId = shopId;
        ImageUrls = imageUrls;
    }

    public void Update(string name, string description, decimal price, string? imageUrls)
    {
        Name = name;
        Description = description;
        Price = price;
        if (imageUrls != null) ImageUrls = imageUrls;
    }

    public void AddStock(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity to add cannot be negative");
        StockQuantity += quantity;
    }

    public bool RemoveStock(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity to remove cannot be negative");
        if (StockQuantity < quantity) return false;
        
        StockQuantity -= quantity;
        return true;
    }
}
