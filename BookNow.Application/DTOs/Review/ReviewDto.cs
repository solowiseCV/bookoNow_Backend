namespace BookNow.Application.DTOs.Review;

public sealed record ReviewDto(
    Guid Id,
    Guid ClientProfileId,
    string ClientName,
    int Rating,
    string Comment,
    DateTime CreatedAt
);
