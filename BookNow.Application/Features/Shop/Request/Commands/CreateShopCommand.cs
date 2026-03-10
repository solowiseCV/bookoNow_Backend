using BookNow.Application.DTOs.Shop;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Commands;

public class CreateShopCommand : IRequest<Result<ShopResponseDto>>
{
    public Guid UserId { get; set; }
    public CreateShopRequestDto RequestDto { get; set; }

    public CreateShopCommand(Guid userId, CreateShopRequestDto requestDto)
    {
        UserId = userId;
        RequestDto = requestDto;
    }
}
