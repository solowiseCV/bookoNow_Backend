using BookNow.Application.Models;

namespace BookNow.Application.Interfaces.Services
{

    public interface IMediaStorageService
    {
       
        Task<string> SaveAsync(MediaFile file, CancellationToken ct);
    }
}