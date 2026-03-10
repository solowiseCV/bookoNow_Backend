using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Shop.Handler.Commands;

public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, Result<ShopResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShopCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ShopResponseDto>> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        
        if (userProfile == null)
        {
            return Result<ShopResponseDto>.Failure("User profile not found.");
        }

        if (userProfile.Role != UserRole.SparePartSeller)
        {
            return Result<ShopResponseDto>.Failure("Only Spare Part Sellers can create shops.");
        }

        var existingShop = await _unitOfWork.Shops.GetByOwnerIdAsync(userProfile.Id, cancellationToken);
        if (existingShop != null)
        {
            return Result<ShopResponseDto>.Failure("Seller already has a shop.");
        }

        var shop = new BookNow.Domain.Entities.Shop(
            request.RequestDto.Name,
            request.RequestDto.Description,
            userProfile.Id,
            request.RequestDto.LogoUrl
        );

        await _unitOfWork.Shops.AddAsync(shop, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new ShopResponseDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            LogoUrl = shop.LogoUrl
        };

        return Result<ShopResponseDto>.Success(responseDto, "Shop created successfully.");
    }
}
