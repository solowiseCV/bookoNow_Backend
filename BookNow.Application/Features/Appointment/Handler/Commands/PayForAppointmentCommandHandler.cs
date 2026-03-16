using BookNow.Application.DTOs.Payment;
using BookNow.Application.Features.Appointment.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;
using Appointment = BookNow.Domain.Entities.Appointment;
using Workshop = BookNow.Domain.Entities.Workshop;

namespace BookNow.Application.Features.Appointment.Request.Commands
{
    public record PayForAppointmentCommand(
        Guid ClientId,
        Guid AppointmentId,
        string Email,
        decimal Amount,
        string CallbackUrl) : IRequest<Result<InitializePaymentResponseDto>>;
}

namespace BookNow.Application.Features.Appointment.Handler.Commands
{
    public class PayForAppointmentCommandHandler : IRequestHandler<PayForAppointmentCommand, Result<InitializePaymentResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaystackService _paystackService;

        public PayForAppointmentCommandHandler(IUnitOfWork unitOfWork, IPaystackService paystackService)
        {
            _unitOfWork = unitOfWork;
            _paystackService = paystackService;
        }

        public async Task<Result<InitializePaymentResponseDto>> Handle(PayForAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment == null) return Result<InitializePaymentResponseDto>.Failure("Appointment not found.");

            var workshop = await _unitOfWork.Workshops.GetByIdAsync(appointment.WorkshopId, cancellationToken);
            if (workshop == null) return Result<InitializePaymentResponseDto>.Failure("Workshop not found.");

            var reference = Guid.NewGuid().ToString("N");

            var paystackRequest = new InitializePaymentRequestDto
            {
                Amount = request.Amount,
                Email = request.Email,
                Reference = reference,
                CallbackUrl = request.CallbackUrl,
                Subaccount = workshop.PaystackSubaccountCode,
                TransactionCharge = (int)Math.Round(request.Amount * 0.05m * 100) // 5% fee
            };

            var response = await _paystackService.InitializePaymentAsync(paystackRequest, cancellationToken);
            if (!response.Status) return Result<InitializePaymentResponseDto>.Failure(response.Message);

            var payment = new BookNow.Domain.Entities.Payment(
                reference,
                request.Amount,
                request.Amount * 0.05m,
                "Paystack",
                BookNow.Domain.Enums.PaymentType.Order, // Can be considered a service order
                null,
                null,
                workshop.Id);

            // Note: Payment entity needs to handle AppointmentId link if required, 
            // but currently we use WorkshopId and OrderId (which is null here).
            // Let's ensure the Payment entity can relate to what we are paying for.

            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<InitializePaymentResponseDto>.Success(response, "Payment initialized.");
        }
    }
}
