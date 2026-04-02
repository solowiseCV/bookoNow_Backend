using BookNow.Application.DTOs.Shop;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Commands;

public class CreateShopCommand(Guid userId, CreateShopRequestDto requestDto, MediaFile? logo = null) : IRequest<Result<ShopResponseDto>>
{
    public Guid UserId { get; set; } = userId;
    public CreateShopRequestDto RequestDto { get; set; } = requestDto;
    public MediaFile? Logo { get; set; } = logo;
}
