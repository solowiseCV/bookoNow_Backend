using System.Security.Claims;
using BookNow.Application.Features.Order.Request.Commands;
using BookNow.Application.Features.Order.Request.Queries;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookNow.Presentation.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrderController(IMediator mediator) : ControllerBase
{
    [SwaggerOperation(Summary = "Creates a new product order for a specific shop")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);

        return Ok(new ApiResponse<object>(
            response.IsSuccess,
            response.Message,
            response.Data
        ));

    }

    [SwaggerOperation(Summary = "Retrieves all orders for the currently authenticated client")]
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]

    public async Task<IActionResult> GetUserOrders(CancellationToken ct)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new ApiResponse<object>(false, "Invalid user token"));
        }

        var response = await mediator.Send(
            new GetUserOrdersQuery(userId),
            ct
        );

        return Ok(new ApiResponse<object>(
            response.IsSuccess,
            response.Message,
            response.Data
        ));
    }
    [SwaggerOperation(Summary = "Retrieves all orders for a specific shop (Shop Owner only)")]
    [HttpGet("shop/{shopId:guid}")]
    [Authorize(Roles = "Shop")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]

    [HttpGet("shop-orders")]
    public async Task<IActionResult> GetShopOrders(CancellationToken ct)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out var ownerId))
        {
            return Unauthorized(new ApiResponse<object>(false, "Invalid user token"));
        }

        var response = await mediator.Send(
            new GetShopOrdersQuery(ownerId),
            ct
        );

        return Ok(new ApiResponse<object>(
            response.IsSuccess,
            response.Message,
            response.Data
        ));
    }
    [SwaggerOperation(Summary = "Updates the status of an order")]
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command, CancellationToken ct)
    {
        command.OrderId = id;
        var response = await mediator.Send(command, ct);

        return Ok(new ApiResponse<object>(
            response.IsSuccess,
            response.Message,
            response.Data
        ));

    }
}
