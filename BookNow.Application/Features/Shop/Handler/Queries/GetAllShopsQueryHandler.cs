using BookNow.Application.DTOs.Shop;
using BookNow.Application.DTOs.Product;
using BookNow.Application.Features.Shop.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Application.Features.Shop.Handler.Queries;

using global::BookNow.Application.Models;



public class GetAllShopsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllShopsQuery, Result<PaginatedResult<ShopResponseDto>>>
{
    public async Task<Result<PaginatedResult<ShopResponseDto>>> Handle(GetAllShopsQuery request, CancellationToken cancellationToken)
    {
        var (shops, totalCount) = await unitOfWork.Shops.GetPaginatedAsync(
            request.PageNumber, 
            request.PageSize, 
            cancellationToken, 
            request.Status,
            request.IsVerified
        );

        var responseList = shops.Select(s => new ShopResponseDto
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
            VerifiedAt = s.VerifiedAt,
            OwnerName = s.Owner?.FullName ?? "Unknown",
            OwnerEmail = s.Owner?.Email ?? "No Email"
        }).ToList();

        var paginatedResult = new PaginatedResult<ShopResponseDto>(
            responseList,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedResult<ShopResponseDto>>.Success(paginatedResult);
    }
}
