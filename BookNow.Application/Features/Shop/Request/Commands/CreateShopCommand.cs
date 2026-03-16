using BookNow.Application.DTOs.Shop;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Commands;

public class CreateShopCommand : IRequest<Result<ShopResponseDto>>
{
    public Guid UserId { get; set; }
    public CreateShopRequestDto RequestDto { get; set; }
    public MediaFile? Logo { get; set; }

    public CreateShopCommand(Guid userId, CreateShopRequestDto requestDto, MediaFile? logo = null)
    {
        UserId = userId;
        RequestDto = requestDto;
        Logo = logo;
    }
}
