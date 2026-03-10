using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Commands.PatchWorkshop;

public sealed record PatchWorkshopCommand(
    Guid Id,
    string? Name = null,
    string? Description = null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    string? PhoneNumber = null,
    string? OpeningHours = null
) : IRequest<Unit>;
