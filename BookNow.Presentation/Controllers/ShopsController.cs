using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Payment.Request.Commands;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllShops([FromQuery] BookNow.Domain.Enums.ShopStatus? status)
    {
        var query = new GetAllShopsQuery(status);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{shopId}/approve")]
    public async Task<IActionResult> ApproveShop(Guid shopId, [FromQuery] bool approve)
    {
        var command = new ApproveShopCommand(shopId, approve);
        var result = await mediator.Send(command);
        
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "SparePartSeller")]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDto request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        // Hardcoded price for Shop Subscription
        decimal amount = 10000; 

        var command = new InitializePaymentCommand(
            null, 
            request.ShopId, 
            null, 
            BookNow.Domain.Enums.PaymentType.Subscription, 
            request.Email, 
            amount, 
            request.CallbackUrl);

        var result = await mediator.Send(command);
        
        if (!result.IsSuccess) return BadRequest(result);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "SparePartSeller")]
    [HttpPost("register-subaccount")]
    public async Task<IActionResult> RegisterSubaccount([FromQuery] string bankName, [FromQuery] string bankCode, [FromQuery] string accountNumber, [FromQuery] string accountName)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var command = new RegisterShopSubaccountCommand(userId, bankName, bankCode, accountNumber, accountName);
        var result = await mediator.Send(command);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}


