using BookNow.Application.Features.Notification.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Notification.Handler.Commands;

public class MarkNotificationAsReadCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<MarkNotificationAsReadCommand, Result<string>>
{
    public async Task<Result<string>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);
        
        if (notification == null) return Result<string>.Failure("Notification not found.");
        
        if (notification.UserId != request.UserId) return Result<string>.Failure("Unauthorized to mark this notification as read.");

        notification.MarkAsRead();
        
        unitOfWork.Notifications.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success("Notification marked as read.", "Success");
    }
}
