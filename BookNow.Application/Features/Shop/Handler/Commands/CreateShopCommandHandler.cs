using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using BookNow.Application.Interfaces.Services;
using MediatR;

namespace BookNow.Application.Features.Shop.Handler.Commands;

public class CreateShopCommandHandler(IUnitOfWork unitOfWork, IMediaStorageService mediaStorage) : IRequestHandler<CreateShopCommand, Result<ShopResponseDto>>
{

    public async Task<Result<ShopResponseDto>> Handle(CreateShopCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        
        if (userProfile == null)
        {
            return Result<ShopResponseDto>.Failure("User profile not found.");
        }

        if (userProfile.Role != UserRole.SparePartSeller)
        {
            return Result<ShopResponseDto>.Failure("Only Spare Part Sellers can create shops.");
        }

        var existingShop = await unitOfWork.Shops.GetByOwnerIdAsync(userProfile.Id, cancellationToken);
        if (existingShop != null)
        {
            return Result<ShopResponseDto>.Failure("Seller already has a shop.");
        }

        string? logoUrl = null;
        if (request.Logo != null)
        {
            logoUrl = await mediaStorage.SaveAsync(request.Logo, cancellationToken);
        }
        if (request.RequestDto.Name is null)
        {
            throw new ArgumentException("Name is required");
        }
        var shop = new BookNow.Domain.Entities.Shop(
            request.RequestDto.Name,
            request.RequestDto.Description,
            request.RequestDto.Address,
            userProfile.Id,
            logoUrl,
            request.RequestDto.PhoneNumber,
            request.RequestDto.OpeningHours
        );

        await unitOfWork.Shops.AddAsync(shop, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        

        var responseDto = new ShopResponseDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            Address = shop.Address,
            PhoneNumber = shop.PhoneNumber!,
            OpeningHours = shop.OpeningHours!,
            LogoUrl = shop.LogoUrl!,
            Status = shop.Status.ToString(),
            IsSubscribed = shop.IsSubscribed,
            VerifiedAt = shop.VerifiedAt,
            OwnerName = userProfile.FullName,
            OwnerEmail = userProfile.Email,
            TargetProfileId = shop.OwnerId
        };

        return Result<ShopResponseDto>.Success(responseDto, "Shop created successfully.");
    }
}
