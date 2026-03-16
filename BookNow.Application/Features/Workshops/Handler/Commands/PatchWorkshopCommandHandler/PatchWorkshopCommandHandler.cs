using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Workshop = BookNow.Domain.Entities.Workshop;

namespace BookNow.Application.Features.Workshops.Handler.Commands.PatchWorkshopCommandHandler;

public sealed class PatchWorkshopCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ILogger<PatchWorkshopCommandHandler> logger) : IRequestHandler<PatchWorkshopCommand, Unit>
{
    public async Task<Unit> Handle(PatchWorkshopCommand request, CancellationToken ct)
    {
        logger.LogInformation("Processing PatchWorkshopCommand for WorkshopId: {WorkshopId}", request.Id);

        if (!Guid.TryParse(currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct) ?? throw new ForbiddenAccessException("User profile not found.");
        Domain.Entities.Workshop? workshop = await unitOfWork.Workshops.GetByIdAsync(request.Id, ct) ?? throw new KeyNotFoundException("Workshop not found.");

        // Verify ownership or Admin role
        if (workshop.MechanicProfileId != userProfile.Id && currentUser.Role != UserRole.Admin.ToString())
        {
            logger.LogWarning("PatchWorkshopCommand unauthorized effort: User {UserId} tried to update workshop {WorkshopId}", userId, request.Id);
            throw new ForbiddenAccessException("You don't have permission to update this workshop.");
        }

        // Partial updates logic
        var name = request.Name is not null ? request.Name.Trim() : workshop.Name;
        var description = request.Description is not null ? request.Description.Trim() : workshop.Description;
        var address = request.Address is not null ? request.Address.Trim() : workshop.Address;
        var latitude = request.Latitude ?? workshop.Latitude;
        var longitude = request.Longitude ?? workshop.Longitude;
        var phoneNumber = request.PhoneNumber is not null ? request.PhoneNumber.Trim() : workshop.PhoneNumber;
        var openingHours = request.OpeningHours is not null ? request.OpeningHours.Trim() : workshop.OpeningHours;
        var type = request.Type ?? workshop.Type;
        var heroImageUrl = request.HeroImageUrl ?? workshop.HeroImageUrl;

        workshop.UpdateDetails(name, description, address, latitude, longitude, phoneNumber, openingHours, type, heroImageUrl);

        unitOfWork.Workshops.Update(workshop);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Workshop {WorkshopId} patched successfully", request.Id);
        return Unit.Value;
    }
}

