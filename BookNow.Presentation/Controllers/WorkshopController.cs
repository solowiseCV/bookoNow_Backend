using BookNow.Application.DTOs.Workshop;
using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;
using BookNow.Application.Features.Workshops.Request.Commands.DeleteWorkshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using BookNow.Application.Models;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("workshops")]
[Authorize]
public class WorkshopController(ISender _sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkshops([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] double? minRating = null)
    {
        var query = new GetWorkshopsQuery(pageNumber, pageSize, minRating);
        var result = await _sender.Send(query);
        return Ok(new ApiResponse<PaginatedResult<WorkshopDto>>(true, "Workshops retrieved successfully", result));
    }
   
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateWorkshop([FromForm] CreateWorkshopRequest request)
    {
        var heroImage = await ToMediaFile(request.HeroImage);
        var galleryImages = request.GalleryImages != null 
            ? await Task.WhenAll(request.GalleryImages.Select(ToMediaFile))
            : null;
      
        var command = new CreateWorkshopCommand(
            Name: request.Name,
            Description: request.Description,
            Address: request.Address,
            Latitude: request.Latitude,
            Longitude: request.Longitude,
            PhoneNumber: request.PhoneNumber,
            OpeningHours: request.OpeningHours,
            HeroImage: heroImage,
            GalleryImages: galleryImages
        );
        
        var workshopId = await _sender.Send(command);
        return CreatedAtAction(nameof(GetWorkshop), new { id = workshopId }, new { id = workshopId });
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkshop(Guid id)
    {
        var query = new GetWorkshopByIdQuery(id);
        var workshop = await _sender.Send(query);
        
        if (workshop == null)
            return NotFound(new ApiResponse(false, "Workshop not found"));

        return Ok(new ApiResponse<WorkshopDto>(true, "Workshop retrieved successfully", workshop));
    }

    private static async Task<MediaFile> ToMediaFile(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return new MediaFile(file.FileName, ms.ToArray(), file.ContentType);
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearbyWorkshops(
        [FromQuery] double latitude,
        [FromQuery] double longitude, 
        [FromQuery] double radiusKm = 10)
    {
        var query = new GetNearbyWorkshopsQuery(latitude, longitude, radiusKm);
        var workshops = await _sender.Send(query);
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
            OpeningHours: request.OpeningHours
        );

        await _sender.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorkshop(Guid id)
    {
        var command = new DeleteWorkshopCommand(id);
        await _sender.Send(command);
        return NoContent();
    }
}
