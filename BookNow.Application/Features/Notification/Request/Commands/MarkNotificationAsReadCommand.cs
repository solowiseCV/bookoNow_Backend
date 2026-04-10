using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Notification.Request.Commands;

public class MarkNotificationAsReadCommand : IRequest<Result<string>>
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
}
