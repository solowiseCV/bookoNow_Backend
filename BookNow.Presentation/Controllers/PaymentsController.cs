using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Common.Options;
using Swashbuckle.AspNetCore.Annotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

using BookNow.Application.DTOs.Order;
using BookNow.Presentation.Models;

namespace BookNow.Presentation.Controllers;

[Route("payments")]
[ApiController]
public class PaymentsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly PaystackOptions _paystackOptions;

    public PaymentsController(IMediator mediator, IOptions<PaystackOptions> paystackOptions)
    {
        _mediator = mediator;
        _paystackOptions = paystackOptions.Value;
    }

    // ─── Paystack Webhook ────────────────────────────────────────────────────────

    [SwaggerOperation(Summary = "Webhook endpoint for Paystack to notify the system of successful payments")]
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PaystackWebhook()
    {
        var secretKey = _paystackOptions.SecretKey;

        var signature = Request.Headers["x-paystack-signature"].ToString();
        using var reader = new StreamReader(Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        var computedSignature = ComputeSha512Hash(requestBody, secretKey);

        if (signature != computedSignature)
            return BadRequest("Invalid signature.");

        try
        {
            var document = System.Text.Json.JsonDocument.Parse(requestBody);
            var root = document.RootElement;

            if (root.TryGetProperty("event", out var eventProperty) && eventProperty.GetString() == "charge.success")
            {
                var data = root.GetProperty("data");
                if (data.TryGetProperty("reference", out var referenceProperty))
                {
                    var reference = referenceProperty.GetString();
                    if (!string.IsNullOrEmpty(reference))
                        await _mediator.Send(new VerifyPaymentCommand(reference));
                }
            }

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Error parsing webhook");
        }
    }

    // ─── Order Payments ──────────────────────────────────────────────────────────

    [SwaggerOperation(Summary = "Initiates a payment process for an existing order. Restricted to Clients")]
    [Authorize(Roles = "Client")]
    [HttpPost("orders/{orderId}/pay")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitiateOrderPayment(Guid orderId, [FromBody] InitiatePaymentRequestDto request)
    {
        var command = new InitializePaymentCommand(
            orderId,
            null,
            null,
            BookNow.Domain.Enums.PaymentType.Order,
            request.Email,
            request.Amount,
            request.CallbackUrl);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    // ─── Appointment Payments ────────────────────────────────────────────────────

    [SwaggerOperation(Summary = "Initiates a payment process for an appointment")]
    [Authorize]
    [HttpPost("appointments/{id}/pay")]
    [ProducesResponseType(typeof(ApiResponse<BookNow.Application.DTOs.Payment.InitializePaymentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PayForAppointment(Guid id, [FromBody] PayForAppointmentRequest request)
    {
        var command = new BookNow.Application.Features.Appointment.Request.Commands.PayForAppointmentCommand(
            UserId, id, request.Email, request.Amount, request.CallbackUrl);

        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    // ─── Shop Subscription Payments ──────────────────────────────────────────────

    [SwaggerOperation(Summary = "Initiates a subscription payment process for a shop. Restricted to SparePartSellers")]
    [Authorize(Roles = "SparePartSeller")]
    [HttpPost("shops/subscribe")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] BookNow.Application.DTOs.Shop.SubscribeRequestDto request)
    {
        var command = new InitializeSubscriptionCommand(UserId, request.ShopId, null, request.Email, request.CallbackUrl);
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────────

    private string ComputeSha512Hash(string input, string key)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
