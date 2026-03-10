using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Workshops.Request.Commands.DeleteWorkshop;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookNow.Application.Features.Workshops.Handler.Commands.DeleteWorkshopCommandHandler;

public sealed class DeleteWorkshopCommandHandler : IRequestHandler<DeleteWorkshopCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteWorkshopCommandHandler> _logger;

    public DeleteWorkshopCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILogger<DeleteWorkshopCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteWorkshopCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Processing DeleteWorkshopCommand: {WorkshopId}", request.Id);

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
        {
            throw new ForbiddenAccessException("Invalid user identity.");
        }

        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
        if (userProfile is null)
        {
            throw new ForbiddenAccessException("User profile not found.");
        }

        var workshop = await _unitOfWork.Workshops.GetByIdAsync(request.Id, ct);
        if (workshop is null)
        {
            throw new KeyNotFoundException("Workshop not found.");
        }

        // Verify ownership
        if (workshop.MechanicProfileId != userProfile.Id && _currentUser.Role != UserRole.Admin.ToString())
        {
            _logger.LogWarning("DeleteWorkshopCommand failed: User {UserId} does not own workshop {WorkshopId}.", userId, request.Id);
            throw new ForbiddenAccessException("You do not have permission to delete this workshop.");
        }

        _unitOfWork.Workshops.Remove(workshop);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Workshop {WorkshopId} deleted successfully.", request.Id);

        return Unit.Value;
    }
}

