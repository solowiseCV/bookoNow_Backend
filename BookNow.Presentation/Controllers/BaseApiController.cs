using BookNow.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

public abstract class BaseApiController : ControllerBase
{
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
