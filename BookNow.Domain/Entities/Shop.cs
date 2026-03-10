

namespace BookNow.Domain.Entities;

public class Shop : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public Guid OwnerId { get; private set; }
    public UserProfile Owner { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Shop() { } 

    public Shop(string name, string description, Guid ownerId, string? logoUrl = null)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        LogoUrl = logoUrl;
    }

    public void Update(string name, string description, string? logoUrl)
    {
        Name = name;
        Description = description;
        if (logoUrl != null) LogoUrl = logoUrl;
    }
}
