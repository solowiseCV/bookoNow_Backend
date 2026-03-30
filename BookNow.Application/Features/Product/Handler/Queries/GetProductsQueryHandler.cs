using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Handler.Queries;

public class GetProductsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductResponseDto>>>
{
    public async Task<Result<PaginatedResult<ProductResponseDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await unitOfWork.Products.SearchAsync(
            request.PageNumber,
            request.PageSize,
            request.Search,
            request.Brand,
            request.Model,
            request.MinPrice,
            request.MaxPrice,
            request.ShopId,
            cancellationToken);

        var productDtos = items.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            ImageUrls = string.IsNullOrEmpty(p.ImageUrls) ? new List<string>() : p.ImageUrls.Split(',').ToList(),
            Model = p.Model,
            Year = p.Year,
            Brand = p.Brand,
            ShopId = p.ShopId
        }).ToList();

        var result = new PaginatedResult<ProductResponseDto>(productDtos, totalCount, request.PageNumber, request.PageSize);

        return Result<PaginatedResult<ProductResponseDto>>.Success(result);
    }
}
