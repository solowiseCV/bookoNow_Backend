using BookNow.Application.DTOs.Order;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Request.Queries;

public class GetShopOrdersQuery : IRequest<Result<IEnumerable<OrderResponseDto>>>
{
    public Guid OwnerId { get; set; }

    public GetShopOrdersQuery(Guid ownerId)
    {
        OwnerId = ownerId;
    }
}
