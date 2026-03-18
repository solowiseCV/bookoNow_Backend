using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IMediator mediator) : BaseApiController
{
    [SwaggerOperation(Summary = "Retrieves all shops, optionally filtered by status. Restricted to Admins")]
    [HttpGet("shops")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShops([FromQuery] BookNow.Domain.Enums.ShopStatus? status)
    {
        var result = await mediator.Send(new GetAllShopsQuery(status));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Approves or rejects a shop. Restricted to Admins")]
    [HttpPatch("shops/{shopId}/approve")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveShop(Guid shopId, [FromQuery] bool approve)
    {
        var result = await mediator.Send(new ApproveShopCommand(shopId, approve));
        return HandleResult(result);
    }
}
