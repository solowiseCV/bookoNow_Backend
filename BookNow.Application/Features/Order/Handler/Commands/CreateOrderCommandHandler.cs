using BookNow.Application.DTOs.Order;
using BookNow.Application.DTOs.Payment;
using BookNow.Application.Features.Order.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using BookNow.Domain.Entities;
using MediatR;

namespace BookNow.Application.Features.Order.Handler.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaystackService _paystackService;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IPaystackService paystackService)
    {
        _unitOfWork = unitOfWork;
        _paystackService = paystackService;
    }

    public async Task<Result<CreateOrderResponseDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var buyerProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.UserId, cancellationToken);
        if (buyerProfile == null) return Result<CreateOrderResponseDto>.Failure("Buyer profile not found.");

        if (request.RequestDto.Items == null || !request.RequestDto.Items.Any())
            return Result<CreateOrderResponseDto>.Failure("Order must contain at least one item.");

        if (string.IsNullOrEmpty(request.RequestDto.Email))
            return Result<CreateOrderResponseDto>.Failure("Buyer email is required for payment initialization.");

        // var order = new BookNow.Domain.Entities.Order(buyerProfile.Id, request.RequestDto.ShippingAddress);
        var order = new BookNow.Domain.Entities.Order(buyerProfile.Id);

        foreach (var itemDto in request.RequestDto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null) return Result<CreateOrderResponseDto>.Failure($"Product {itemDto.ProductId} not found.");

            if (!product.RemoveStock(itemDto.Quantity))
                return Result<CreateOrderResponseDto>.Failure($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}");

            order.AddItem(product.Id, itemDto.Quantity, product.Price);
            _unitOfWork.Products.Update(product);
        }

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        
        // Save initial order
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Initialize Paystack Payment
        var reference = Guid.NewGuid().ToString("N");
        
        // For simplicity and to ensure correct split, we assume products in an order are from the same shop
        // In a real multi-seller app, we might split orders per seller or use multi-split.
        var firstProductId = request.RequestDto.Items.First().ProductId;
        var productForShop = await _unitOfWork.Products.GetByIdAsync(firstProductId, cancellationToken);
        var shop = await _unitOfWork.Shops.GetByIdAsync(productForShop!.ShopId, cancellationToken);

        var paystackRequest = new InitializePaymentRequestDto
        {
            Amount = order.TotalAmount,
            Email = request.RequestDto.Email,
            Reference = reference,
            CallbackUrl = "https://yourfrontend.com/payments/verify", // Can be passed from frontend
            Subaccount = shop?.PaystackSubaccountCode,
            TransactionCharge = (int)Math.Round(order.TotalAmount * 0.05m * 100) // 5% fee in kobo
        };

        var paystackResponse = await _paystackService.InitializePaymentAsync(paystackRequest, cancellationToken);

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

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
