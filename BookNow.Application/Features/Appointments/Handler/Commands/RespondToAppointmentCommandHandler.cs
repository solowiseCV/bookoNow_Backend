using BookNow.Application.Features.Appointments.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Appointments.Handler.Commands;

public class RespondToAppointmentCommandHandler(
    IUnitOfWork unitOfWork
    ) : IRequestHandler<RespondToAppointmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RespondToAppointmentCommand request, CancellationToken cancellationToken)
    {
        var identityUser = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (identityUser == null)
            return Result<string>.Failure("User profile not found.");

        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment == null)
            return Result<string>.Failure("Appointment not found.");

        var workshop = await unitOfWork.Workshops.GetByIdAsync(appointment.WorkshopId, cancellationToken);
        if (workshop == null || workshop.MechanicProfileId != identityUser.Id)
            return Result<string>.Failure("You do not have permission to respond to this appointment.");

        var clientUser = await unitOfWork.UserProfiles.GetByIdAsync(appointment.ClientProfileId, cancellationToken);

        if (request.Accept)
        {
            appointment.Accept();
            unitOfWork.Appointments.Update(appointment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify Client
            if (clientUser != null)
            {
                // We don't have direct access to phone number here without Identity lookup, so we pass null or try to retrieve it if stored.
                var message = $"Your appointment at {workshop.Name} scheduled for {appointment.AppointmentAt} has been accepted.";
                // await notificationService.SendNotificationAsync(clientUser.IdentityUserId, null, message, cancellationToken);
            }

            return Result<string>.Success("Appointment accepted.", "Success");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                return Result<string>.Failure("Rejection reason is required when declining an appointment.");

            appointment.Reject(request.RejectionReason);
            unitOfWork.Appointments.Update(appointment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify Client
            if (clientUser != null)
            {
                var message = $"Your appointment at {workshop.Name} has been declined. Reason: {request.RejectionReason}";
                // await notificationService.SendNotificationAsync(clientUser.IdentityUserId, null, message, cancellationToken);
            }

            return Result<string>.Success("Appointment declined.", "Success");
        }
    }
}
