using BookNow.Application.DTOs.Product;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Request.Commands;

public class AddProductCommand(Guid userId, AddProductRequestDto requestDto, List<MediaFile>? images = null) : IRequest<Result<ProductResponseDto>>
{
    public Guid UserId { get; set; } = userId;
    public AddProductRequestDto RequestDto { get; set; } = requestDto;
    public List<MediaFile>? Images { get; set; } = images;
}
