using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Payment.Request.Commands;

public record InitializeSubscriptionCommand(
    Guid UserId,
    Guid? ShopId,
    Guid? WorkshopId,
    string Email,
    string CallbackUrl) : IRequest<Result<string>>;
