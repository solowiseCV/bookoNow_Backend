using BookNow.Domain.Enums;

namespace BookNow.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid CustomerId { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        public decimal TotalAmount { get; private set; }

        public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

        public Payment? Payment { get; private set; }

        private Order() { }

        public Order(Guid customerId)
        {
            CustomerId = customerId;
            TotalAmount = 0;
            Status = OrderStatus.Pending;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            var item = new OrderItem(Id, productId, quantity, unitPrice);
            Items.Add(item);

            TotalAmount += item.TotalPrice;
        }

        public void UpdateStatus(OrderStatus status)
        {
            Status = status;
        }
    }
}