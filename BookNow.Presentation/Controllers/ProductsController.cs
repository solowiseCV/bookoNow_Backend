using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Commands;
using BookNow.Application.Features.Product.Request.Queries;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("products")]
[ApiController]
public class ProductsController(IMediator mediator) : BaseApiController
{
    [SwaggerOperation(Summary = "Retrieves all products with optional filtering and pagination")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var result = await mediator.Send(query);
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Retrieves a product by its unique identifier")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return HandleResult(result);
    }
   
    [SwaggerOperation(Summary = "Adds a new product to the shop. Restricted to users with 'SparePartSeller' role")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProduct([FromForm] AddProductRequest request)
    {
        var requestDto = new AddProductRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Year = request.Year,
            Model = request.Model,
            Brand = request.Brand 
        };

        var images = await ToMediaFiles(request.Images);
        var result = await mediator.Send(new AddProductCommand(UserId, requestDto, images));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Updates an existing product. Restricted to users with 'SparePartSeller' role")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductRequest request)
    {
        var requestDto = new AddProductRequestDto
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Year = request.Year,
            Model = request.Model,
            Brand = request.Brand 
        };

        var newImages = await ToMediaFiles(request.Images);
        var result = await mediator.Send(new UpdateProductCommand(id, UserId, requestDto, newImages, request.ImageUrlsToKeep));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Deletes a product. Restricted to users with 'SparePartSeller' role")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id, UserId));
        return HandleResult(result);
    }
}
