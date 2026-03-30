using BookNow.Application.DTOs.Product;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Product.Request.Queries;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? Brand = null,
    string? Model = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    Guid? ShopId = null) : IRequest<Result<PaginatedResult<ProductResponseDto>>>;
