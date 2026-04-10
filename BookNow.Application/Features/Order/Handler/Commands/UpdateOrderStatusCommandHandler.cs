using BookNow.Application.Features.Order.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Order.Handler.Commands;

public class UpdateOrderStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IBackgroundJobService backgroundJobService
    ) : IRequestHandler<UpdateOrderStatusCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var identityUser = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.IdentityUserId, cancellationToken);
        if (identityUser == null)
            return Result<string>.Failure("User profile not found.");

        var order = await unitOfWork.Orders.GetWithItemsByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            return Result<string>.Failure("Order not found.");

        // Check if any item in the order is associated with a product whose shop is owned by this user.
        bool isOwner = order.Items.Any(oi => oi.Product?.Shop?.OwnerId == identityUser.Id);

        if (!isOwner)
        {
            return Result<string>.Failure("You do not have permission to update this order.");
        }

        order.UpdateStatus(request.Status);
        unitOfWork.Orders.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Notifications for Shipped or Cancelled statuses
        if (request.Status == OrderStatus.Shipped || request.Status == OrderStatus.Cancelled)
        {
            var buyerProfile = await unitOfWork.UserProfiles.GetByIdAsync(order.CustomerId, cancellationToken);
            if (buyerProfile != null)
            {
                var statusLabel = request.Status == OrderStatus.Shipped ? "shipped" : "cancelled";
                var message = $"Your order (ID: {order.Id.ToString()[..8]}) has been {statusLabel}.";
                
                backgroundJobService.Enqueue<INotificationService>(service => 
                    service.SendNotificationAsync(buyerProfile.IdentityUserId, buyerProfile.PhoneNumber, message, CancellationToken.None));

                backgroundJobService.Enqueue<IEmailService>(service => 
                    service.SendNotificationEmailAsync(buyerProfile.Email, $"Order {statusLabel.ToUpper()}", "Order Update", 
                        $"Hello {buyerProfile.FullName}, your order with ID {order.Id} has been marked as {statusLabel} by the seller.", 
                        "View Order", "https://booknow-three.vercel.app/orders"));
            }
        }

        return Result<string>.Success($"Order status updated to {request.Status}.", "Success");
    }
}
