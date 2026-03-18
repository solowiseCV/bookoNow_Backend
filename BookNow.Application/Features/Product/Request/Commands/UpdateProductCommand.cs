using BookNow.Application.DTOs.Product;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Request.Commands;

public record UpdateProductCommand(
    Guid ProductId,
    Guid UserId,
    AddProductRequestDto RequestDto,
    List<MediaFile> NewImages,
    List<string>? ImageUrlsToKeep = null) : IRequest<Result<Unit>>;
