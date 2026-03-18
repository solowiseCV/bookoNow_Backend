using BookNow.Application.Features.Reviews.Request.Commands;
using Swashbuckle.AspNetCore.Annotations;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("reviews")]
[Authorize]
public class ReviewController(ISender _sender) : BaseApiController
{
 
    [SwaggerOperation(Summary = "Creates a new review for an appointment")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        var command = new CreateReviewCommand(
            request.AppointmentId,
            request.Rating,
            request.Comment
        );

        var reviewId = await _sender.Send(command);
        return CreatedAtAction(null, new { id = reviewId }, new ApiResponse<Guid>(true, "Review created successfully", reviewId));
    }

   
    [SwaggerOperation(Summary = "Updates an existing review")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
    {
        var command = new UpdateReviewCommand(id, request.Rating, request.Comment);
        await _sender.Send(command);
        return Ok(new ApiResponse(true, "Review updated successfully"));
    }

 
    [SwaggerOperation(Summary = "Deletes a review")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var command = new DeleteReviewCommand(id);
        await _sender.Send(command);
        return Ok(new ApiResponse(true, "Review deleted successfully"));
    }
}

public record CreateReviewRequest(Guid AppointmentId, int Rating, string Comment);
public record UpdateReviewRequest(int Rating, string Comment);
