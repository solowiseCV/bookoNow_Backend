using BookNow.Application.DTOs.Workshop;
using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Queries;
public sealed record GetNearbyWorkshopsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm = 10,
    bool? IsVerified = null
) : IRequest<IReadOnlyList<WorkshopDto>>;

