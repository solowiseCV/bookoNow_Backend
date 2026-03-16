using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Payment : BaseEntity
{
    public string Reference { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public decimal SystemCommission { get; private set; } 
    public PaymentStatus Status { get; private set; }
    public PaymentType Type { get; private set; }
    public string Gateway { get; private set; } = default!;
    
    public Guid? OrderId { get; private set; }
    public Order? Order { get; private set; }

    public Guid? ShopId { get; private set; }
    public Shop? Shop { get; private set; }

    public Guid? WorkshopId { get; private set; }
    public Workshop? Workshop { get; private set; }

    private Payment() { }

    public Payment(string reference, decimal amount, decimal systemCommission, string gateway, PaymentType type = PaymentType.Order, Guid? orderId = null, Guid? shopId = null, Guid? workshopId = null)
    {
        Reference = reference;
        Amount = amount;
        SystemCommission = systemCommission;
        Gateway = gateway;
        Type = type;
        OrderId = orderId;
        ShopId = shopId;
        WorkshopId = workshopId;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsSuccessful()
    {
        Status = PaymentStatus.Success;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
    }
}
