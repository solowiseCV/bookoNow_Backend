using Swashbuckle.AspNetCore.Annotations;
using BookNow.Application.Features.Order.Request.Commands;
using BookNow.Application.Features.Order.Request.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

using BookNow.Presentation.Models;
using BookNow.Application.DTOs.Order;

namespace BookNow.Presentation.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController(IMediator mediator) : BaseApiController
{
 
    [SwaggerOperation(Summary = "Creates a new order for spare parts. Restricted to users with 'Client' role")]
    [Authorize(Roles = "Client")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var appRequestDto = new BookNow.Application.DTOs.Order.CreateOrderRequestDto 
        { 
            ShippingAddress = request.ShippingAddress, 
            Items = request.Items.Select(i => new BookNow.Application.DTOs.Order.OrderItemRequestDto { ProductId = i.ProductId, Quantity = i.Quantity }).ToList(),
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
        };
        var result = await mediator.Send(new CreateOrderCommand(UserId, appRequestDto));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Gets orders for the authenticated shop owner. Restricted to users with 'SparePartSeller' role")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpGet("shop")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetShopOrders()
    {
        var result = await mediator.Send(new GetShopOrdersQuery(UserId));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Updates the status of an order. Restricted to users with 'SparePartSeller' role")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        var result = await mediator.Send(new UpdateOrderStatusCommand(id, request.Status, UserId));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Gets orders for the authenticated client. Restricted to users with 'Client' role")]
    [Authorize(Roles = "Client")]
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserOrders()
    {
        var result = await mediator.Send(new GetUserOrdersQuery(UserId));
        return HandleResult(result);
    }
}
