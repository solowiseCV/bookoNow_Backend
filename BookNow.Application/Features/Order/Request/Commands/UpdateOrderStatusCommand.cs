using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Order.Request.Commands;

public class UpdateOrderStatusCommand : IRequest<Result<string>>
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public Guid IdentityUserId { get; set; }

    public UpdateOrderStatusCommand(Guid orderId, OrderStatus status, Guid identityUserId)
    {
        OrderId = orderId;
        Status = status;
        IdentityUserId = identityUserId;
    }
}
