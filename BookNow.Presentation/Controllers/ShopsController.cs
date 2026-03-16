using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Features.Shop.Request.Queries;
using Microsoft.Extensions.Options;
using BookNow.Application.Common.Options;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopsController(IMediator mediator, IUnitOfWork unitOfWork, IOptions<PaystackOptions> paystackOptions) : BaseApiController
{
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost]
    public async Task<IActionResult> CreateShop([FromForm] CreateShopRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var requestDto = new CreateShopRequestDto
        {
            Name = request.Name,
            Description = request.Description
        };

        var logo = request.Logo != null ? await ToMediaFile(request.Logo) : null;
        var command = new CreateShopCommand(userId, requestDto, logo);
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

        var shop = await unitOfWork.Shops.GetByIdAsync(request.ShopId, HttpContext.RequestAborted);
        if (shop == null) return NotFound("Shop not found.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, HttpContext.RequestAborted);
        if (userProfile == null || shop.OwnerId != userProfile.Id)
        {
            return Forbid("You do not own this shop.");
        }

        if (shop.IsSubscribed)
        {
            return BadRequest("Shop is already subscribed.");
        }

        decimal amount = paystackOptions.Value.ShopSubscriptionFee;
        if (amount <= 0) amount = 10000; // Fallback

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
        return Ok(result);
    }

    [Authorize(Roles = "SparePartSeller")]
    [HttpPost("register-subaccount")]
    public async Task<IActionResult> RegisterSubaccount([FromQuery] Guid shopId, [FromQuery] string bankName, [FromQuery] string bankCode, [FromQuery] string accountNumber, [FromQuery] string accountName)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var command = new RegisterShopSubaccountCommand(userId, shopId, bankName, bankCode, accountNumber, accountName);
        var result = await mediator.Send(command);

        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
