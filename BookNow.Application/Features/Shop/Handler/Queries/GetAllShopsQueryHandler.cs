using BookNow.Application.DTOs.Shop;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

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

        var response = shops.Select(s => new ShopResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            LogoUrl = s.LogoUrl,
            Status = s.Status.ToString(),
            IsSubscribed = s.IsSubscribed,
            VerifiedAt = s.VerifiedAt
        });

        return Result<IEnumerable<ShopResponseDto>>.Success(response);
    }
}
