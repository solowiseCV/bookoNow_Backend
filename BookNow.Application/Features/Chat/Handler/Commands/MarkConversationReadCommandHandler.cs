using BookNow.Application.Features.Chat.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Handler.Commands;

public sealed class MarkConversationReadCommandHandler : IRequestHandler<MarkConversationReadCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealTimeChatService _realTimeChatService;

    public MarkConversationReadCommandHandler(
        IUnitOfWork unitOfWork,
        IRealTimeChatService realTimeChatService)
    {
        _unitOfWork = unitOfWork;
        _realTimeChatService = realTimeChatService;
    }

    public async Task<Result<bool>> Handle(MarkConversationReadCommand request, CancellationToken ct)
    {
        if (request.ViewerIdentityUserId == Guid.Empty)
            return Result<bool>.Failure("Invalid authenticated user.");

        if (request.ConversationId == Guid.Empty)
            return Result<bool>.Failure("Conversation id is required.");

        var viewerProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.ViewerIdentityUserId, ct);
        if (viewerProfile == null)
            return Result<bool>.Failure("User profile not found.");

        var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(request.ConversationId, ct);
        if (conversation == null)
            return Result<bool>.Failure("Conversation not found.");

        if (!conversation.Participants.Any(p => p.ProfileId == viewerProfile.Id))
            return Result<bool>.Failure("You are not a participant in this conversation.");

        var unreadMessages = await _unitOfWork.Messages.GetUnreadMessagesForConversationAsync(request.ConversationId, viewerProfile.Id, ct);
        if (!unreadMessages.Any())
            return Result<bool>.Success(true, "No unread messages to mark as read.");

        foreach (var message in unreadMessages)
        {
            message.MarkAsRead();
            _unitOfWork.Messages.Update(message);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        var payload = new
        {
            ConversationId = request.ConversationId,
            MessageIds = unreadMessages.Select(m => m.Id).ToArray(),
            ReadAt = DateTime.UtcNow
        };

        var otherParticipantIdentityIds = conversation.Participants
            .Where(p => p.ProfileId != viewerProfile.Id)
            .Select(p => p.Profile.IdentityUserId)
            .Distinct();

        foreach (var identityUserId in otherParticipantIdentityIds)
        {
            await _realTimeChatService.PushMessagesReadAsync(identityUserId, payload, ct);
        }

        return Result<bool>.Success(true, "Conversation marked as read.");
    }
}
