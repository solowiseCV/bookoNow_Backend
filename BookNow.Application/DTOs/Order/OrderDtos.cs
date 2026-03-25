namespace BookNow.Application.DTOs.Order;

public class CreateOrderRequestDto
{
    public List<OrderItemRequestDto> Items { get; set; } = new();
    public string? ShippingAddress { get; set; }
    // buyer email is provided by the controller from the authenticated user's claims
    public string Email { get; set; } = string.Empty;
}

public class OrderItemRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderResponseDto
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentReference { get; set; } = null!;
    public string AuthorizationUrl { get; set; } = null!;
}

// public class CreateOrderRequestDto
// {
//     public string ShippingAddress { get; set; } = null!;
//     public List<OrderItemRequestDto> Items { get; set; } = new();
// }

// public class OrderItemRequestDto
// {
//     public Guid ProductId { get; set; }
//     public int Quantity { get; set; }
//     public decimal UnitPrice { get; set; }
// }

public class InitiatePaymentRequestDto
{
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public string CallbackUrl { get; set; } = null!;
}

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public BookNow.Domain.Enums.OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class UpdateOrderStatusRequestDto
{
    public BookNow.Domain.Enums.OrderStatus Status { get; set; }
}
