using BookNow.Application.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BookNow.Presentation.Controllers;

[Route("api/test-notifications")]
[ApiController]
public class NotificationTestController(
    IEmailService emailService,
    INotificationService notificationService,
    IBackgroundJobClient backgroundJobClient) : ControllerBase
{
    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail([FromQuery] string email)
    {
        await emailService.SendNotificationEmailAsync(
            email,
            "Test Notification from BookNow",
            "Infrastructure Test",
            "This is a test email to verify that our new template system and MailKit integration are working correctly.",
            "View Dashboard",
            "https://booknownow.vercel.app/dashboard"
        );

        return Ok(new { message = $"Test email sent to {email}" });
    }

    [HttpPost("test-signalr")]
    public async Task<IActionResult> TestSignalR([FromQuery] Guid userId, [FromQuery] string message)
    {
        await notificationService.SendNotificationAsync(userId, null, message, default);
        return Ok(new { message = $"SignalR notification pushed to user {userId}" });
    }

    /// <summary>
    /// Enqueues a Hangfire job that always throws an exception.
    /// Use this to verify the global retry policy (3 attempts) in the Hangfire Dashboard.
    /// After 3 failures the job will appear in the "Failed" tab.
    /// </summary>
    [HttpPost("test-fail-job")]
    public IActionResult TestFailJob()
    {
        var jobId = backgroundJobClient.Enqueue(() => AlwaysFailingJob());
        return Accepted(new
        {
            message = "Deliberate-fail job enqueued. Open /hangfire to observe 3 retry attempts.",
            jobId
        });
    }

    /// <summary>
    /// This method is intentionally broken so Hangfire retries it 3 times before marking it Failed.
    /// It must be public and static-friendly so Hangfire can serialise the invocation.
    /// </summary>
    [NonAction]
    public static void AlwaysFailingJob()
    {
        throw new InvalidOperationException(
            "[TEST] Deliberate failure — verifying Hangfire retry limit of 3 attempts.");
    }
}
