using BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Workshops.Handler.Commands.CreateWorkshopCommandHandler;

public sealed class CreateWorkshopCommandHandler
    : IRequestHandler<CreateWorkshopCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateWorkshopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateWorkshopCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);

        if (userProfile is null)
        {
             throw new UnauthorizedAccessException("User profile not found.");
        }

        if (_currentUser.Role != UserRole.Mechanic.ToString())
            throw new InvalidOperationException("Only mechanics can create workshops.");

        var workshop = new Workshop(
            mechanicProfileId: userProfile.Id,
            name: request.Name,
            description: request.Description,
            address: request.Address,
            latitude: request.Latitude,
            longitude: request.Longitude
        );

        await _unitOfWork.Workshops.AddAsync(workshop, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return workshop.Id;
    }
}

