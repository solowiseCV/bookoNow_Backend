using BookNow.Application.Features.Payment.Request.Commands;
using BookNow.Application.Common.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace BookNow.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PaystackOptions _paystackOptions;

    public PaymentsController(IMediator mediator, IOptions<PaystackOptions> paystackOptions)
    {
        _mediator = mediator;
        _paystackOptions = paystackOptions.Value;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> PaystackWebhook()
    {
        var secretKey = _paystackOptions.SecretKey;

        // Verify Signature
        var signature = Request.Headers["x-paystack-signature"].ToString();
        using var reader = new StreamReader(Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        var computedSignature = ComputeSha512Hash(requestBody, secretKey);
        
        if (signature != computedSignature)
        {
            return BadRequest("Invalid signature.");
        }

        // Parse Event
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
                    {
                        var command = new VerifyPaymentCommand(reference);
                        await _mediator.Send(command);
                    }
                }
            }

            return Ok(); // Acknowledge receipt
        }
        catch (Exception)
        {
            return BadRequest("Error parsing webhook");
        }
    }

    private string ComputeSha512Hash(string input, string key)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
