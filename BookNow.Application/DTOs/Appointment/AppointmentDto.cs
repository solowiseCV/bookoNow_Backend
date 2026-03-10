using BookNow.Domain.Enums;

namespace BookNow.Application.DTOs.Appointment;

public sealed record AppointmentDto(
    Guid Id,
    Guid ClientProfileId,
    Guid WorkshopId,
    DateTime AppointmentAt,
    AppointmentStatus Status,
    string IssueDescription,
    IReadOnlyList<AppointmentAttachmentDto> Attachments
);
