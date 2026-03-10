
using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities;

public class Order : BaseEntity
{
    public Guid BuyerId { get; private set; }
    public UserProfile Buyer { get; private set; }

    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? ShippingAddress { get; private set; }

    public ICollection<OrderItem> orderItems = new List<OrderItem>();
    public IReadOnlyCollection<OrderItem> OrderItems => orderItems.ToList().AsReadOnly();

    public Payment? Payment { get; private set; }

    private Order() { }

    public Order(Guid buyerId, string? shippingAddress)
    {
        BuyerId = buyerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = new OrderItem(Id, productId, quantity, unitPrice);
        orderItems.Add(item);
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        TotalAmount = orderItems.Sum(x => x.TotalPrice);
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}
