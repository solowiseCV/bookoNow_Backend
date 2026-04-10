using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshop.Handler.Commands;
using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.DeleteWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Models;
using BookNow.Domain.Enums;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("workshops")]
[Authorize]

public class WorkshopController(ISender _sender) : BaseApiController
{
   
    [SwaggerOperation(Summary = "Retrieves a paginated list of workshops with optional filtering")]
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<WorkshopDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkshops(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] double? minRating = null,
        [FromQuery] string? search = null,
        [FromQuery] WorkshopType? type = null,
        [FromQuery] bool? isVerified = null)
    {
        var result = await _sender.Send(new GetWorkshopsQuery(pageNumber, pageSize, minRating, search, type, isVerified));
        return Ok(new ApiResponse<PaginatedResult<WorkshopDto>>(true, "Workshops retrieved successfully", result));
    }
   
   
    [SwaggerOperation(Summary = "Creates a new workshop with hero and gallery images")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWorkshop([FromForm] CreateWorkshopRequest request)
    {
        var heroImage = request.HeroImage != null ? await ToMediaFile(request.HeroImage) : null;
        var galleryImages = await ToMediaFiles(request.GalleryImages);

        var command = new CreateWorkshopCommand(
            Name: request.Name,
            Description: request.Description,
            Address: request.Address,
            PhoneNumber: request.PhoneNumber ?? string.Empty,
            OpeningHours: request.OpeningHours ?? string.Empty,
            Type: request.Type,
            HeroImage: heroImage,
            GalleryImages: [.. galleryImages]
        );

        var workshopId = await _sender.Send(command);
        return CreatedAtAction(nameof(GetWorkshop), new { id = workshopId }, new ApiResponse<Guid>(true, "Workshop created successfully", workshopId));
    }


    [SwaggerOperation(Summary = "Retrieves a workshop by its unique identifier")]
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WorkshopDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkshop(Guid id)
    {
        var result = await _sender.Send(new GetWorkshopByIdQuery(id));
        if (result is null) return NotFound(new ApiResponse(false, "Workshop not found"));
        return Ok(new ApiResponse<WorkshopDto>(true, "Workshop retrieved successfully", result));
    }

    [SwaggerOperation(Summary = "Retrieves workshops within a specified radius of a given location")]
    [HttpGet("nearby")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WorkshopDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNearbyWorkshops(
        [FromQuery] double latitude,
        [FromQuery] double longitude, 
        [FromQuery] double radiusKm = 10,
        [FromQuery] bool? isVerified = null)
    {
        var workshops = await _sender.Send(new GetNearbyWorkshopsQuery(latitude, longitude, radiusKm, isVerified));
        return Ok(new ApiResponse<IReadOnlyList<WorkshopDto>>(true, "Nearby workshops retrieved successfully", workshops));
    }

 
    [SwaggerOperation(Summary = "Retrieves all workshops owned by the currently authenticated mechanic")]
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WorkshopDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyWorkshops()
    {
        var result = await _sender.Send(new GetMyWorkshopsQuery());
        return Ok(new ApiResponse<IReadOnlyList<WorkshopDto>>(true, "Your workshops retrieved successfully", result));
    }

    [SwaggerOperation(Summary = "Updates specific fields of an existing workshop")]
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> PatchWorkshop(Guid id, [FromBody] PatchWorkshopRequest request)
    {
        var command = new PatchWorkshopCommand(
            Id: id,
            Name: request.Name,
            Description: request.Description,
            Address: request.Address,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            PhoneNumber: request.PhoneNumber,
            OpeningHours: request.OpeningHours,
            Type: request.Type,
            HeroImageUrl: request.HeroImageUrl
        );

        await _sender.Send(command);
        return Ok(new ApiResponse(true, "Workshop updated successfully"));
    }

  
    [SwaggerOperation(Summary = "Deletes a workshop by its unique identifier")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteWorkshop(Guid id)
    {
        await _sender.Send(new DeleteWorkshopCommand(id));
        return Ok(new ApiResponse(true, "Workshop deleted successfully"));
    }

    [SwaggerOperation(Summary = "Registers a bank subaccount for a workshop to enable payments")]
    [Authorize]
    [HttpPost("register-subaccount")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterSubaccount([FromBody] RegisterSubaccountRequestDto request)
    {
        var result = await _sender.Send(new RegisterWorkshopSubaccountCommand(UserId, request.WorkshopId, request.BankName, request.BankCode, request.AccountNumber, request.AccountName));
        return HandleResult(result);
    }
}