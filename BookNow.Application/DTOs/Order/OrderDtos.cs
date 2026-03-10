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
