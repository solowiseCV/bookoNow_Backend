using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Features.Workshop.Request.Commands;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Features.Admin.Queries;
using BookNow.Application.DTOs.Admin;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.DTOs.Shop;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IMediator mediator) : BaseApiController
{
    [SwaggerOperation(Summary = "Retrieves all shops, optionally filtered by status. Restricted to Admins")]
    [HttpGet("shops")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ShopResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllShops(
        [FromQuery] BookNow.Domain.Enums.ShopStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllShopsQuery(status, pageNumber, pageSize));
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

    [SwaggerOperation(Summary = "Verifies a workshop. Restricted to Admins")]
    [HttpPatch("workshops/{workshopId}/verify")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyWorkshop(Guid workshopId)
    {
        var result = await mediator.Send(new VerifyWorkshopCommand(workshopId));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Retrieves platform statistics. Restricted to Admins")]
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<AdminStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var result = await mediator.Send(new GetAdminStatsQuery());
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Retrieves all users. Restricted to Admins")]
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Retrieves paginated workshops for administration. Restricted to Admins")]
    [HttpGet("workshops")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<WorkshopDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkshopsAdmin(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        // For workshops, we'll use the existing GetWorkshopsQuery but with admin-level includes 
        // that we added to the repository.
        var result = await mediator.Send(new GetWorkshopsQuery(pageNumber, pageSize));
        return HandleResult(Result<PaginatedResult<WorkshopDto>>.Success(result));
    }
}
