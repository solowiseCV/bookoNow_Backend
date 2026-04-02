using BookNow.Application.DTOs.Order;
using BookNow.Application.Features.Order.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Handler.Queries;

public class GetShopOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetShopOrdersQuery, Result<IEnumerable<OrderResponseDto>>>
{
    public async Task<Result<IEnumerable<OrderResponseDto>>> Handle(GetShopOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await unitOfWork.Orders.GetOrdersByShopOwnerIdAsync(request.OwnerId, cancellationToken);

        var orderDtos = orders.Select(o => new OrderResponseDto
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            Status = o.Status,
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,
            Items = o.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        });

        return Result<IEnumerable<OrderResponseDto>>.Success(orderDtos, "Shop orders retrieved successfully.");
    }
}
