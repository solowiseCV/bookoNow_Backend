

namespace BookNow.Application.DTOs.Workshop;
public sealed record WorkshopDto(
    Guid Id,
    string Name,
    string Description,
    string Address,
    double Latitude,
    double Longitude,
    bool IsVerified
);


