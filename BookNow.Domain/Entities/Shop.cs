
using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Shop : BaseEntity
{
    public  string Name { get; private set; } =string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public Guid OwnerId { get; private set; }
    public UserProfile Owner { get; private set; } = default!;

    public ShopStatus Status { get; private set; } = ShopStatus.Pending;
    public bool IsSubscribed { get; private set; } = false;
    public DateTime? VerifiedAt { get; private set; }
    public string? PaystackSubaccountCode { get; private set; }
    public string? BankName { get; private set; }
    public string? BankCode { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? AccountName { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Shop() { } 

    public Shop(string name, string description, Guid ownerId, string? logoUrl = null)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        LogoUrl = logoUrl;
        Status = ShopStatus.Pending;
    }

    public void Update(string name, string description, string? logoUrl)
    {
        Name = name;
        Description = description;
        if (logoUrl != null) LogoUrl = logoUrl;
    }

    public void Approve()
    {
        Status = ShopStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = ShopStatus.Rejected;
    }

    public void SetSubscription(bool subscribed)
    {
        IsSubscribed = subscribed;
    }

    public void SetBankDetails(string bankName, string bankCode, string accountNumber, string accountName)
    {
        BankName = bankName;
        BankCode = bankCode;
        AccountNumber = accountNumber;
        AccountName = accountName;
    }

    public void SetPaystackSubaccount(string subaccountCode)
    {
        PaystackSubaccountCode = subaccountCode;
    }
}
