using BookNow.Application.DTOs.Notification;
using BookNow.Application.Features.Notification.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Notification.Handler.Queries;

public class GetNotificationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetNotificationsQuery, Result<List<NotificationResponseDto>>>
{
    public async Task<Result<List<NotificationResponseDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await unitOfWork.Notifications.GetUserNotificationsAsync(request.UserId, cancellationToken);
        
        var dtos = notifications
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToList();

        return Result<List<NotificationResponseDto>>.Success(dtos, "Notifications retrieved successfully.");
    }
}
