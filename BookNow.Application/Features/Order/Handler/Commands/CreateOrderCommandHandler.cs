using BookNow.Application.DTOs.Order;
using BookNow.Application.DTOs.Payment;
using BookNow.Application.Features.Order.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Order.Handler.Commands;

public class CreateOrderCommandHandler(
    IUnitOfWork unitOfWork, 
    IPaystackService paystackService,
    IBackgroundJobService backgroundJobService) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponseDto>>
{
    public async Task<Result<CreateOrderResponseDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var buyerProfile = await unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (buyerProfile == null) return Result<CreateOrderResponseDto>.Failure("Buyer profile not found.");

        if (request.RequestDto.Items == null || request.RequestDto.Items.Count == 0)
            return Result<CreateOrderResponseDto>.Failure("Order must contain at least one item.");

        if (string.IsNullOrEmpty(request.RequestDto.Email))
            return Result<CreateOrderResponseDto>.Failure("Buyer email is required for payment initialization.");

        // var order = new BookNow.Domain.Entities.Order(buyerProfile.Id, request.RequestDto.ShippingAddress);
        var order = new BookNow.Domain.Entities.Order(buyerProfile.Id);

        foreach (var itemDto in request.RequestDto.Items)
        {
            var product = await unitOfWork.Products.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null) return Result<CreateOrderResponseDto>.Failure($"Product {itemDto.ProductId} not found.");

            if (!product.RemoveStock(itemDto.Quantity))
                return Result<CreateOrderResponseDto>.Failure($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}");

            order.AddItem(product.Id, itemDto.Quantity, product.Price);
            unitOfWork.Products.Update(product);
        }

        await unitOfWork.Orders.AddAsync(order, cancellationToken);
        
        // Save initial order
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Initialize Paystack Payment
        var reference = Guid.NewGuid().ToString("N");
        
        // For simplicity and to ensure correct split, we assume products in an order are from the same shop
        // In a real multi-seller app, we might split orders per seller or use multi-split.
        var firstProductId = request.RequestDto.Items.First().ProductId;
        var productForShop = await unitOfWork.Products.GetByIdAsync(firstProductId, cancellationToken);
        var shop = await unitOfWork.Shops.GetByIdAsync(productForShop!.ShopId, cancellationToken);

        if (shop != null)
        {
            var owner = await unitOfWork.UserProfiles.GetByIdAsync(shop.OwnerId, cancellationToken);
            if (owner != null)
            {
                var message = $"A new order has been placed for products in your shop '{shop.Name}'. Ordered Amount: ₦{order.TotalAmount:N2}.";
                
                // Notify Shop Owner via Background Job
                backgroundJobService.Enqueue<INotificationService>(service => 
                    service.SendNotificationAsync(owner.IdentityUserId, owner.PhoneNumber, message, CancellationToken.None));

                backgroundJobService.Enqueue<IEmailService>(service => 
                    service.SendNotificationEmailAsync(owner.Email, "New Order Received", "New Order", 
                        $"Hello {owner.FullName}, you have received a new order at your shop '{shop.Name}'. Total: ₦{order.TotalAmount:N2}.", 
                        "Manage Orders", "https://booknow-three.vercel.app/dashboard/orders"));
            }
        }

        // Notify Buyer
        var buyerMessage = $"Your order (ID: {order.Id.ToString()[..8]}) has been created. Please complete the payment to proceed.";
        backgroundJobService.Enqueue<INotificationService>(service => 
            service.SendNotificationAsync(buyerProfile.IdentityUserId, buyerProfile.PhoneNumber, buyerMessage, CancellationToken.None));

        backgroundJobService.Enqueue<IEmailService>(service => 
            service.SendNotificationEmailAsync(buyerProfile.Email, "Order Created", "Pending Payment", 
                $"Hello {buyerProfile.FullName}, your order with ID {order.Id} has been created. \n\nTotal: ₦{order.TotalAmount:N2}.", 
                "Complete Payment", "https://booknow-three.vercel.app/orders"));

        var paystackRequest = new InitializePaymentRequestDto
        {
            Amount = order.TotalAmount,
            Email = request.RequestDto.Email,
            Reference = reference,
            CallbackUrl = "https://yourfrontend.com/payments/verify", // Can be passed from frontend
            Subaccount = shop?.PaystackSubaccountCode,
            TransactionCharge = (int)Math.Round(order.TotalAmount * 0.05m * 100) // 5% fee in kobo
        };

        var paystackResponse = await paystackService.InitializePaymentAsync(paystackRequest, cancellationToken);

        if (!paystackResponse.Status || paystackResponse.Data == null)
            return Result<CreateOrderResponseDto>.Failure("Failed to initialize payment with Paystack.");

        // Record Payment attempt
        var commission = order.TotalAmount * 0.05m; // 5% Commission
        var payment = new BookNow.Domain.Entities.Payment(
            paystackResponse.Data.Reference, 
            order.TotalAmount, 
            commission, 
            "Paystack", 
            BookNow.Domain.Enums.PaymentType.Order, 
            order.Id);

        await unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = new CreateOrderResponseDto
        {
            OrderId = order.Id,
            TotalAmount = order.TotalAmount,
            PaymentReference = paystackResponse.Data.Reference,
            AuthorizationUrl = paystackResponse.Data.AuthorizationUrl
        };

        return Result<CreateOrderResponseDto>.Success(responseDto, "Order created and payment initialized.");
    }
}
