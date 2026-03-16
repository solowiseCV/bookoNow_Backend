using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : BaseApiController
{
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] AddProductRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var requestDto = new AddProductRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var images = await ToMediaFiles(request.Images);
        var command = new AddProductCommand(userId, requestDto, images);
        var result = await mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
