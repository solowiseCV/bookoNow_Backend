using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Features.Shop.Request.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopsController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost]
    public async Task<IActionResult> CreateShop([FromBody] CreateShopRequestDto requestDto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var command = new CreateShopCommand(userId, requestDto);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetShopByOwner), new { ownerId = userId }, result);
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetShopByOwner(Guid ownerId)
    {
        var query = new GetShopByOwnerIdQuery(ownerId);
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
