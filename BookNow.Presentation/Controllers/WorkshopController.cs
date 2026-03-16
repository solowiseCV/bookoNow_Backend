using System.Security.Claims;
using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.DeleteWorkshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Models;
using BookNow.Domain.Enums;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Application.Features.Workshop.Handler.Commands;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("workshops")]
[Authorize]

public class WorkshopController(ISender _sender) : BaseApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkshops(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] double? minRating = null,
        [FromQuery] string? search = null,
        [FromQuery] WorkshopType? type = null)
    {
        var query = new GetWorkshopsQuery(pageNumber, pageSize, minRating, search, type);
        var result = await _sender.Send(query);
        return Ok(new ApiResponse<PaginatedResult<WorkshopDto>>(true, "Workshops retrieved successfully", result));
    }
   
    [HttpPost]
    public async Task<IActionResult> CreateWorkshop([FromForm] CreateWorkshopRequest request)
    {
        var heroImage = request.HeroImage != null ? await ToMediaFile(request.HeroImage) : null;
        var galleryImages = await ToMediaFiles(request.GalleryImages);

        var command = new CreateWorkshopCommand(
            Name: request.Name,
            Description: request.Description,
            Address: request.Address,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            PhoneNumber: request.PhoneNumber ?? string.Empty,
            OpeningHours: request.OpeningHours ?? string.Empty,
            Type: request.Type,
            HeroImage: heroImage,
            GalleryImages: [.. galleryImages]
        );

        var workshopId = await _sender.Send(command);

        return CreatedAtAction(nameof(GetWorkshop), new { id = workshopId }, new { id = workshopId });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkshop(Guid id)
    {
        var workshop = await _sender.Send(new GetWorkshopByIdQuery(id));
        if (workshop is null)
            return NotFound(new ApiResponse(false, "Workshop not found"));

        return Ok(new ApiResponse<WorkshopDto>(true, "Workshop retrieved successfully", workshop));
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearbyWorkshops(
        [FromQuery] double latitude,
        [FromQuery] double longitude, 
        [FromQuery] double radiusKm = 10)
    {
        var workshops = await _sender.Send(new GetNearbyWorkshopsQuery(latitude, longitude, radiusKm));
        return Ok(new ApiResponse<IReadOnlyList<WorkshopDto>>(true, "Nearby workshops retrieved successfully", workshops));
    }

    [HttpPatch("{id:guid}")]
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
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorkshop(Guid id)
    {
        await _sender.Send(new DeleteWorkshopCommand(id));
        return NoContent();
    }

    [Authorize]
    [HttpPost("register-subaccount")]
    public async Task<IActionResult> RegisterSubaccount([FromBody] RegisterSubaccountRequestDto request)
       
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(new ApiResponse(false, "Invalid user token"));
           
            if (string.IsNullOrWhiteSpace(request.BankName) ||
            string.IsNullOrWhiteSpace(request.BankCode) ||
            string.IsNullOrWhiteSpace(request.AccountNumber) ||
            string.IsNullOrWhiteSpace(request.AccountName))
        {
            return BadRequest(new ApiResponse(false, "All bank details are required."));
        }
        var result = await _sender.Send(new RegisterWorkshopSubaccountCommand(userId, request.WorkshopId, request.BankName, request.BankCode, request.AccountNumber, request.AccountName));

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}