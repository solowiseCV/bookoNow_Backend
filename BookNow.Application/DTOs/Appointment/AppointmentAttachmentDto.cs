using BookNow.Domain.Entities;

namespace BookNow.Application.DTOs.Appointment;

public sealed record AppointmentAttachmentDto(
    Guid Id,
    string Url,
    MediaType Type
);
