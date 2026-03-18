using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Product.Request.Commands;

public record DeleteProductCommand(Guid ProductId, Guid UserId) : IRequest<Result<Unit>>;
