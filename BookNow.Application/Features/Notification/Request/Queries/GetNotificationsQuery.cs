using BookNow.Application.DTOs.Notification;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Notification.Request.Queries;

public class GetNotificationsQuery : IRequest<Result<List<NotificationResponseDto>>>
{
    public Guid UserId { get; set; }
}
