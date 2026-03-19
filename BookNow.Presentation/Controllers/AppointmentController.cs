using MediatR;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookNow.Application.Models;
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Features.Appointments.Request.Queries;

using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("appointments")]
[Authorize]
public class AppointmentController(ISender _sender) : BaseApiController
{
  
    [SwaggerOperation(Summary = "Creates a new appointment with optional media files")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(104_857_600)]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAppointment([FromForm] Models.CreateAppointmentRequest request)
    {
        var mediaFiles = await ToMediaFiles(request.MediaFiles);

        var command = new CreateAppointmentCommand(
            request.WorkshopId,
            request.AppointmentAt,
            request.IssueDescription,
            mediaFiles);

        var appointmentId = await _sender.Send(command);
        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, new ApiResponse<Guid>(true, "Appointment created successfully", appointmentId));
    }

    [SwaggerOperation(Summary = "Retrieves an appointment by its unique identifier")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointmentById(Guid id)
    {
        var dto = await _sender.Send(new GetAppointmentByIdQuery(id));
        return Ok(new ApiResponse<object>(true, "Appointment retrieved successfully", dto));
    }

 
    [SwaggerOperation(Summary = "Retrieves all appointments for a specific workshop")]
    [HttpGet("workshop/{workshopId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByWorkshop(Guid workshopId)
    {
        var list = await _sender.Send(new GetAppointmentsByWorkshopQuery(workshopId));
        return Ok(new ApiResponse<object>(true, "Workshop appointments retrieved successfully", list));
    }

 
    [SwaggerOperation(Summary = "Retrieves all appointments for the currently authenticated client")]
    [HttpGet("my-appointments")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAppointments()
    {
        var list = await _sender.Send(new GetAppointmentsByClientQuery());
        return Ok(new ApiResponse<object>(true, "Your appointments retrieved successfully", list));
    }

    [SwaggerOperation(Summary = "Accepts or rejects an appointment with an optional reason")]
    [HttpPatch("{id}/respond")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RespondToAppointment(Guid id, [FromBody] RespondToAppointmentRequest request)
    {
        var result = await _sender.Send(new RespondToAppointmentCommand(id, request.Accept, request.RejectionReason, UserId));
        return HandleResult(result);
    }


    [SwaggerOperation(Summary = "Updates an existing appointment")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] Models.UpdateAppointmentRequest request)
    {
        await _sender.Send(new UpdateAppointmentCommand(
            id,
            request.AppointmentAt,
            request.IssueDescription));
        return Ok(new ApiResponse(true, "Appointment updated successfully"));
    }


    [SwaggerOperation(Summary = "Deletes an appointment")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        await _sender.Send(new DeleteAppointmentCommand(id));
        return Ok(new ApiResponse(true, "Appointment deleted successfully"));
    }

}

public class RespondToAppointmentRequest
{
    public bool Accept { get; set; }
    public string? RejectionReason { get; set; }
}
