using BookNow.Application.Models;
using BookNow.Domain.Common;
using BookNow.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookNow.Presentation.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId) ? Guid.Empty : userId;
        }
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(new ApiResponse<T>(true, result.Message, result.Data!));

        return BadRequest(new ApiResponse(false, result.Message));
    }
    protected static async Task<MediaFile> ToMediaFile(IFormFile file)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return new MediaFile(file.FileName, ms.ToArray(), file.ContentType);
    }

    protected static async Task<List<MediaFile>> ToMediaFiles(IEnumerable<IFormFile>? files)
    {
        if (files == null) return [];
        var tasks = files.Select(ToMediaFile);
        var mediaFiles = await Task.WhenAll(tasks);
        return [.. mediaFiles];
    }
}
