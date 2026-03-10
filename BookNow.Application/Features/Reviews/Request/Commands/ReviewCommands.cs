using MediatR;

namespace BookNow.Application.Features.Reviews.Request.Commands;

public sealed record CreateReviewCommand(
    Guid AppointmentId,
    int Rating,
    string Comment
) : IRequest<Guid>;

public sealed record UpdateReviewCommand(
    Guid Id,
    int Rating,
    string Comment
) : IRequest<Unit>;

public sealed record DeleteReviewCommand(Guid Id) : IRequest<Unit>;
