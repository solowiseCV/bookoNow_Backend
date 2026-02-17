using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Features.Workshops.Request.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("workshops")]
[Authorize]
public class WorkshopController(ISender _sender) : ControllerBase
{
   
    [HttpPost]
    public async Task<IActionResult> CreateWorkshop([FromBody] CreateWorkshopCommand command)
    {
        var workshopId = await _sender.Send(command);
        return CreatedAtAction(nameof(CreateWorkshop), new { id = workshopId }, new { id = workshopId });
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
        return Ok(workshops);
    }
}
