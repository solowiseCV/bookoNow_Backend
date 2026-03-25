using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Appointments.Request.Commands;

public class RespondToAppointmentCommand : IRequest<Result<string>>
{
    public Guid AppointmentId { get; set; }
    public bool Accept { get; set; }
    public string? RejectionReason { get; set; }
    public Guid IdentityUserId { get; set; } // The user making the request

    public RespondToAppointmentCommand(Guid appointmentId, bool accept, string? rejectionReason, Guid identityUserId)
    {
        AppointmentId = appointmentId;
        Accept = accept;
        RejectionReason = rejectionReason;
        IdentityUserId = identityUserId;
    }
}
