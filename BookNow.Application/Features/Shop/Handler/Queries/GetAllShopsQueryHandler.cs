using BookNow.Application.DTOs.Shop;
using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.Features.Shop.Handler.Queries;

public class GetAllShopsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllShopsQuery, Result<IEnumerable<ShopResponseDto>>>
{
    public async Task<Result<IEnumerable<ShopResponseDto>>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken)
    {
        var shops = await unitOfWork.Shops.GetAllAsync(cancellationToken);
    
        
        if (request.Status.HasValue)
        {
            shops = shops.Where(s => s.Status == request.Status.Value);
        }

        var responseList = new List<ShopResponseDto>();

        foreach (var s in shops)
        {
            var shopDto = new ShopResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                LogoUrl = s.LogoUrl ?? "",
                Address = s.Address,
                PhoneNumber = s.PhoneNumber ?? "",
                OpeningHours = s.OpeningHours ?? "",
                Status = s.Status.ToString(),
                IsSubscribed = s.IsSubscribed,
                VerifiedAt = s.VerifiedAt
            };

            var products = await unitOfWork.Products.GetByShopIdAsync(s.Id, cancellationToken);
            shopDto.Products = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrls = p.ImageUrls,
                Model = p.Model,
                Year = p.Year,
                Brand = p.Brand,
                ShopId = p.ShopId
            }).ToList();

            responseList.Add(shopDto);
        }

        return Result<IEnumerable<ShopResponseDto>>.Success(responseList);
    }
}
