using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookNow.Presentation.Controllers;
[Route("shops")]
[ApiController]
public class ShopsController(IMediator mediator) : BaseApiController
{

    [SwaggerOperation(Summary = "Creates a new shop for a spare part seller")]
    [Authorize(Roles ="SparePartSeller")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShop([FromForm] CreateShopRequest request)
    {
        if (UserId == Guid.Empty) return Unauthorized(new ApiResponse(false, "User ID not found in token."));

        var requestDto = new CreateShopRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber,
            OpeningHours = request.OpeningHours
        };

        var logo = request.Logo != null ? await ToMediaFile(request.Logo) : null;
        var command = new BookNow.Application.Features.Shop.Request.Commands.CreateShopCommand(UserId, requestDto, logo);
        var result = await mediator.Send(command);

        if (!result.IsSuccess) return HandleResult(result);

        return CreatedAtAction(nameof(GetShopById), new { id = result.Data }, new ApiResponse<object>(true, "Shop created successfully", result));
    }


    [SwaggerOperation(Summary = "Retrieves a shop by its unique identifier")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShopById(Guid id)
    {
        var result = await mediator.Send(new GetShopByIdQuery(id));
        return HandleResult(result);
    }


    [SwaggerOperation(Summary = "Retrieves a shop by the owner's unique identifier")]
    [HttpGet("owner/{ownerId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShopByOwner(Guid ownerId)
    {
        var result = await mediator.Send(new BookNow.Application.Features.Shop.Request.Queries.GetShopByOwnerIdQuery(ownerId));
        return HandleResult(result);
    }


    [SwaggerOperation(Summary = "Registers a bank subaccount for a shop to enable payments")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost("register-subaccount")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterSubaccount([FromQuery] Guid shopId, [FromQuery] string bankName, [FromQuery] string bankCode, [FromQuery] string accountNumber, [FromQuery] string accountName)
    {
        var command = new BookNow.Application.Features.Shop.Request.Commands.RegisterShopSubaccountCommand(UserId, shopId, bankName, bankCode, accountNumber, accountName);
        var result = await mediator.Send(command);
        return HandleResult(result);
    }
}
