using BookNow.Application.DTOs.Order;
using BookNow.Application.Features.Order.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Handler.Queries;

public class GetUserOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserOrdersQuery, Result<IEnumerable<OrderResponseDto>>>
{
    public async Task<Result<IEnumerable<OrderResponseDto>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var identityUser = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (identityUser == null)
            return Result<IEnumerable<OrderResponseDto>>.Failure("User profile not found.");

        var orders = await unitOfWork.Orders.GetByBuyerIdAsync(identityUser.Id, cancellationToken);

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

        return Result<IEnumerable<OrderResponseDto>>.Success(orderDtos, "User orders retrieved successfully.");
    }
}
