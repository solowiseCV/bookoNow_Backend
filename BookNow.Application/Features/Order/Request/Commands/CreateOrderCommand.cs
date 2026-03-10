using BookNow.Application.DTOs.Order;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Request.Commands;

public class CreateOrderCommand : IRequest<Result<CreateOrderResponseDto>>
{
    public Guid UserId { get; set; } // The Identity user id of the buyer
    public CreateOrderRequestDto RequestDto { get; set; }

    public CreateOrderCommand(Guid userId, CreateOrderRequestDto requestDto)
    {
        UserId = userId;
        RequestDto = requestDto;
    }
}
