using BookNow.Application.DTOs.Product;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Request.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductResponseDto>>;
