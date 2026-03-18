using Swashbuckle.AspNetCore.Annotations;
using BookNow.Application.Features.Order.Request.Commands;
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

}
