using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Product.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Handler.Queries;

public class GetProductByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductByIdQuery, Result<ProductResponseDto>>
{
    public async Task<Result<ProductResponseDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
        {
            return Result<ProductResponseDto>.Failure("Product not found.");
        }

        var responseDto = new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrls = string.IsNullOrEmpty(product.ImageUrls) ? new List<string>() : product.ImageUrls.Split(',').ToList(),
            Model = product.Model,
            Year = product.Year,
            Brand = product.Brand,
            ShopId = product.ShopId
        };

        return Result<ProductResponseDto>.Success(responseDto);
    }
}
