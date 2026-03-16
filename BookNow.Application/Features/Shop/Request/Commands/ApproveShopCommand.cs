using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Shop.Request.Commands;

public record ApproveShopCommand(Guid ShopId, bool Approve) : IRequest<Result<bool>>;
