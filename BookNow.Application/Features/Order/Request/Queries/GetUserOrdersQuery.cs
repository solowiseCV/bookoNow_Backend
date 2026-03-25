using BookNow.Application.DTOs.Order;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Request.Queries;

public class GetUserOrdersQuery : IRequest<Result<IEnumerable<OrderResponseDto>>>
{
    public Guid UserId { get; set; }

    public GetUserOrdersQuery(Guid userId)
    {
        UserId = userId;
    }
}
