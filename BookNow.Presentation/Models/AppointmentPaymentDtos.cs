namespace BookNow.Presentation.Models;

public class PayForAppointmentRequest
{
    public string Email { get; set; } = null!;
    public decimal Amount { get; set; }
    public string CallbackUrl { get; set; } = null!;
}
