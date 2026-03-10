using BookNow.Application.Interfaces.Services;
using BookNow.Application.Models;
using BookNow.Infrastructure.Identity;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BookNow.Infrastructure.Services
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<MediaStorageService> _logger;

        public MediaStorageService(IConfiguration configuration, ILogger<MediaStorageService> logger)
        {
            _logger = logger;

            var settings = configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
            if (settings == null)
                throw new InvalidOperationException("Cloudinary settings are not configured.");

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> SaveAsync(MediaFile file, CancellationToken ct)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.Content == null || file.Content.Length == 0)
                throw new ArgumentException("File content is empty", nameof(file));

            // upload to cloudinary
            using var ms = new MemoryStream(file.Content);
            UploadResult result;
            if (file.ContentType.StartsWith("video", StringComparison.OrdinalIgnoreCase))
            {
                var videoParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, ms),
                    PublicId = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                };
                result = await _cloudinary.UploadAsync(videoParams);
            }
            else
            {
                var imageParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, ms),
                    PublicId = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                };
                result = await _cloudinary.UploadAsync(imageParams);
            }

            if (result.StatusCode != HttpStatusCode.OK && result.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Cloudinary upload failed: {Error}", result.Error?.Message);
                throw new Exception("Failed to upload media to cloudinary.");
            }

            return result.SecureUrl?.ToString() ?? throw new Exception("Cloudinary returned no url.");
        }
    }
}
