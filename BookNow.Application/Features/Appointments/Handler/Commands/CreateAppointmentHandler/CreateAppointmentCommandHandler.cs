
using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Authentication;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Entities;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands.CreateAppointmentHandler;
public sealed class CreateAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IMediaStorageService mediaStorage,
    IBackgroundJobService backgroundJobService)
        : IRequestHandler<CreateAppointmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        if (currentUser.Role != UserRole.Client.ToString()  && currentUser.Role != UserRole.SparePartSeller.ToString() && currentUser.Role != UserRole.Admin.ToString())
            throw new InvalidOperationException("Only clients, spare part sellers, and admins can book appointments.");

        if (!Guid.TryParse(currentUser.UserId, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(userId, ct);

        if (userProfile is null)
        {
             throw new UnauthorizedAccessException("User profile not found.");
        }

        var appointment = new BookNow.Domain.Entities.Appointment(
            clientProfileId: userProfile.Id,
            workshopId: request.WorkshopId,
            appointmentAt: request.AppointmentAt.ToUniversalTime(),
            issueDescription: request.IssueDescription
        );

        if (request.MediaFiles != null && request.MediaFiles.Any())
        {
            foreach (var file in request.MediaFiles)
            {
                var url = await mediaStorage.SaveAsync(file, ct);
                appointment.AddAttachment(url, MediaType.Image);
            }
        }

        await unitOfWork.Appointments.AddAsync(appointment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, ct);
        if (workshop != null)
        {
            var mechanic = await unitOfWork.UserProfiles.GetByIdAsync(workshop.MechanicProfileId, ct);
            if (mechanic != null)
            {
                var message = $"A new appointment has been booked at your workshop '{workshop.Name}' for {appointment.AppointmentAt:g}.";
                
                // Notify Mechanic via Background Job
                backgroundJobService.Enqueue<INotificationService>(service => 
                    service.SendNotificationAsync(mechanic.IdentityUserId, mechanic.PhoneNumber, message, CancellationToken.None));

                backgroundJobService.Enqueue<IEmailService>(service => 
                    service.SendNotificationEmailAsync(mechanic.Email, "New Appointment Booking", "New Appointment", 
                        $"Hello {mechanic.FullName}, a new appointment has been booked at your workshop '{workshop.Name}' by {userProfile.FullName} for {appointment.AppointmentAt:g}.", 
                        "Manage Appointments", "https://booknow-three.vercel.app/dashboard/appointments"));
            }

            // Notify Client via Background Job
            var clientMessage = $"Your appointment at '{workshop.Name}' for {appointment.AppointmentAt:g} has been successfully booked.";
            backgroundJobService.Enqueue<INotificationService>(service => 
                service.SendNotificationAsync(userId, userProfile.PhoneNumber, clientMessage, CancellationToken.None));

            backgroundJobService.Enqueue<IEmailService>(service => 
                service.SendNotificationEmailAsync(userProfile.Email, "Appointment Booking Confirmation", "Booking Confirmed", 
                    $"Hello {userProfile.FullName}, your appointment at '{workshop.Name}' for {appointment.AppointmentAt:g} has been confirmed. You will receive updates as the mechanic responds to your request.", 
                    "View Appointment", "https://booknow-three.vercel.app/appointments"));
        }

        return appointment.Id;
    }
}
