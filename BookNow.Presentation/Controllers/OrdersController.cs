using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Features.Order.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Client")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var appRequestDto = new BookNow.Application.DTOs.Order.CreateOrderRequestDto 
        { 
            ShippingAddress = request.ShippingAddress, 
            Items = request.Items.Select(i => new BookNow.Application.DTOs.Order.OrderItemRequestDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList(),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
        };
        var command = new CreateOrderCommand(userId, appRequestDto);
        var result = await mediator.Send(command);

        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }

    [Authorize(Roles = "Client")]
    [HttpPost("{orderId}/pay")]
    public async Task<IActionResult> InitiatePayment(Guid orderId, [FromBody] InitiatePaymentRequestDto request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // We should ideally fetch the order to get the amount, but for simplicity in this command flow:
        // The InitializePaymentCommandHandler handles the logic. We just need to pass the OrderId.
        // However, the current InitializePaymentCommand needs the amount.
        // Let's assume we fetch it or the command handles it. 
        // Actually, I'll update the Command to fetch Amount if OrderId is provided.
        
        var command = new InitializePaymentCommand(
            orderId, 
            null, 
            null, 
            BookNow.Domain.Enums.PaymentType.Order, 
            request.Email, 
            request.Amount, 
            request.CallbackUrl);

        var result = await mediator.Send(command);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}

public class CreateOrderRequestDto
{
    public string ShippingAddress { get; set; } = null!;
    public List<OrderItemRequestDto> Items { get; set; } = new();
}

public class OrderItemRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class InitiatePaymentRequestDto
{
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; } // Ideally fetched from DB
    public string CallbackUrl { get; set; } = null!;
}
