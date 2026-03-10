using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookNow.Application.Features.Workshops.Handler.Commands.PatchWorkshopCommandHandler;

public sealed class PatchWorkshopCommandHandler : IRequestHandler<PatchWorkshopCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PatchWorkshopCommandHandler> _logger;

    public PatchWorkshopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<PatchWorkshopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Unit> Handle(PatchWorkshopCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Processing PatchWorkshopCommand for WorkshopId: {WorkshopId}", request.Id);

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
        if (userProfile is null)
            throw new ForbiddenAccessException("User profile not found.");

        var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.Id, ct);
        if (workshop is null)
            throw new KeyNotFoundException("Workshop not found.");

        // Verify ownership or Admin role
        if (workshop.MechanicProfileId != userProfile.Id && _currentUser.Role != UserRole.Admin.ToString())
        {
            _logger.LogWarning("PatchWorkshopCommand unauthorized effort: User {UserId} tried to update workshop {WorkshopId}", userId, request.Id);
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

        workshop.UpdateDetails(name, description, address, latitude, longitude, phoneNumber, openingHours);

        _unitOfWork.Workshops.Update(workshop);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Workshop {WorkshopId} patched successfully", request.Id);
        return Unit.Value;
    }
}

