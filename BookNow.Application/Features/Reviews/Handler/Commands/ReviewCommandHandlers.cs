using BookNow.Application.Common.Exceptions;
using BookNow.Application.Features.Reviews.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookNow.Application.Features.Reviews.Handler.Commands;

public sealed class CreateReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, ILogger<CreateReviewCommandHandler> logger) : IRequestHandler<CreateReviewCommand, Guid>
{
    public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var profile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct) ?? throw new ForbiddenAccessException("User profile not found.");
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, ct) ?? throw new KeyNotFoundException("Appointment not found.");
        if (appointment.ClientProfileId != profile.Id)
            throw new ForbiddenAccessException("You can only review your own appointments.");

        var workshop = await unitOfWork.Workshops.GetByIdAsync(appointment.WorkshopId, ct) ?? throw new KeyNotFoundException("Workshop not found.");
        if (workshop.MechanicProfileId == profile.Id)
            throw new InvalidOperationException("You cannot review your own workshop.");
        
        var review = new Review(
            profile.Id,
            workshop.Id,
            appointment.Id,
            request.Rating,
            request.Comment.Trim()
        );

        await unitOfWork.Reviews.AddAsync(review, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Review {ReviewId} created for workshop {WorkshopId} by user {UserId}", review.Id, workshop.Id, userId);
        return review.Id;
    }
}

public sealed class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var profile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
        if (profile is null)
            throw new ForbiddenAccessException("User profile not found.");

        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, ct);
        if (review is null)
            throw new KeyNotFoundException("Review not found.");

        if (review.ClientProfileId != profile.Id && _currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("You do not have permission to update this review.");

        review.Update(request.Rating, request.Comment.Trim());
        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

public sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteReviewCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            throw new ForbiddenAccessException("Invalid user identity.");

        var profile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);
        if (profile is null)
            throw new ForbiddenAccessException("User profile not found.");

        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, ct);
        if (review is null)
            throw new KeyNotFoundException("Review not found.");

        if (review.ClientProfileId != profile.Id && _currentUser.Role != UserRole.Admin.ToString())
            throw new ForbiddenAccessException("You do not have permission to delete this review.");

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

