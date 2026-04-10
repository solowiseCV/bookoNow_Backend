using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Commands.CreateWorkshopCommandHandler;

public sealed class CreateWorkshopCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMediaStorageService mediaStorage,
    IBackgroundJobService backgroundJobService)
        : IRequestHandler<CreateWorkshopCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkshopCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        if (currentUser.Role != UserRole.Mechanic.ToString())
            throw new ForbiddenAccessException("Only mechanics can create workshops.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);

        if (userProfile is null)
            throw new ForbiddenAccessException("User profile not found.");

        var existingWorkshops = await unitOfWork.Workshops.GetByMechanicAsync(userProfile.Id, ct);
        if (existingWorkshops.Any())
            throw new BadRequestException("You can only have one workshop.");

        string? heroImageUrl = null;

        if (request.HeroImage != null)
        {
            heroImageUrl = await mediaStorage.SaveAsync(request.HeroImage, ct);
        }

        var workshop = new  BookNow.Domain.Entities.Workshop(
            mechanicProfileId: userProfile.Id,
            name: request.Name,
            description: request.Description,
            address: request.Address,
            latitude: request.Latitude,
            longitude: request.Longitude,
            type: request.Type,
            heroImageUrl: heroImageUrl,
            phoneNumber: request.PhoneNumber,
            openingHours: request.OpeningHours
        );

        await unitOfWork.Workshops.AddAsync(workshop, ct);

        if (request.GalleryImages is { Count: > 0 })
        {
            var uploadTasks = request.GalleryImages
                .Select(img => mediaStorage.SaveAsync(img, ct));

            var uploadedUrls = await Task.WhenAll(uploadTasks);

            foreach (var url in uploadedUrls)
            {
                var image = new WorkshopImage(workshop.Id, url);
                workshop.GalleryImages.Add(image);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        // Notify Mechanic
        backgroundJobService.Enqueue<INotificationService>(service => 
            service.SendNotificationAsync(userId, userProfile.PhoneNumber, 
                $"Your workshop '{workshop.Name}' has been submitted for verification.", CancellationToken.None));

        // Send confirmation email
        backgroundJobService.Enqueue<IEmailService>(service => 
            service.SendNotificationEmailAsync(userProfile.Email, "Workshop Submitted", "Workshop Received", 
                $"Hello {userProfile.FullName}, your workshop '{workshop.Name}' has been successfully created and is awaiting admin verification.", 
                "View Dashboard", "https://booknow-three.vercel.app/dashboard"));

        return workshop.Id;
    }
}