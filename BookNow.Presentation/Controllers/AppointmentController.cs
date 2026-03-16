using MediatR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Application.Models;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Features.Appointments.Request.Queries;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("appointments")]
[Authorize]
public class AppointmentController(ISender _sender) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(104_857_600)]
    public async Task<IActionResult> CreateAppointment([FromForm] Models.CreateAppointmentRequest request)
    {
        var mediaFiles = new List<MediaFile>();
        foreach (var formFile in request.MediaFiles)
        {
            if (formFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await formFile.CopyToAsync(ms);
                mediaFiles.Add(new MediaFile(
                    formFile.FileName,
                    ms.ToArray(),
                    formFile.ContentType));
            }
        }

        var command = new CreateAppointmentCommand(
            request.WorkshopId,
            request.AppointmentAt,
            request.IssueDescription,
            mediaFiles);

        var appointmentId = await _sender.Send(command);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, new { id = appointmentId });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        var dto = await _sender.Send(new GetAppointmentByIdQuery(id));
        return Ok(dto);
    }

    [HttpGet("workshop/{workshopId}")]
    public async Task<IActionResult> GetByWorkshop(Guid workshopId)
    {
        var list = await _sender.Send(new GetAppointmentsByWorkshopQuery(workshopId));
        return Ok(list);
    }

    [HttpGet("my-appointments")]
    public async Task<IActionResult> GetMyAppointments()
    {
        var list = await _sender.Send(new GetAppointmentsByClientQuery());
        return Ok(list);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] Models.UpdateAppointmentRequest request)
    {
        await _sender.Send(new UpdateAppointmentCommand(
            id,
            request.AppointmentAt,
            request.IssueDescription));
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        await _sender.Send(new DeleteAppointmentCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayForAppointment(Guid id, [FromBody] Models.PayForAppointmentRequest request)
    {
        var clientIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(clientIdString) || !Guid.TryParse(clientIdString, out var clientId))
        {
            return Unauthorized();
        }

        var command = new BookNow.Application.Features.Appointment.Request.Commands.PayForAppointmentCommand(
            clientId,
            id,
            request.Email,
            request.Amount,
            request.CallbackUrl);

        var result = await _sender.Send(command);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
}
