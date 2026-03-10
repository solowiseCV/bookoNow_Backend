
using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Payment : BaseEntity
{
    public string Reference { get; private set; }
    public decimal Amount { get; private set; }
    public decimal SystemCommission { get; private set; } // 5% by default
    public PaymentStatus Status { get; private set; }
    public string Gateway { get; private set; } // e.g., "Paystack"
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; }

    private Payment() { }

    public Payment(string reference, decimal amount, decimal systemCommission, string gateway, Guid orderId)
    {
        Reference = reference;
        Amount = amount;
        SystemCommission = systemCommission;
        Gateway = gateway;
        OrderId = orderId;
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
