using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;
using System.Linq;

namespace BookNow.Application.Features.Chat.Handler.Commands;

public sealed class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Result<ChatMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealTimeChatService _realTimeChatService;

    public SendChatMessageCommandHandler(IUnitOfWork unitOfWork, IRealTimeChatService realTimeChatService)
    {
        _unitOfWork = unitOfWork;
        _realTimeChatService = realTimeChatService;
    }

    public async Task<Result<ChatMessageDto>> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        if (request.SenderIdentityUserId == Guid.Empty)
            return Result<ChatMessageDto>.Failure("Invalid authenticated user.");

        if (request.ConversationId == Guid.Empty)
            return Result<ChatMessageDto>.Failure("Conversation id is required.");

        if (string.IsNullOrWhiteSpace(request.Content) && string.IsNullOrWhiteSpace(request.ImageUrl))
            return Result<ChatMessageDto>.Failure("Message content or image is required.");

        var senderProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.SenderIdentityUserId, ct);
        if (senderProfile == null)
            return Result<ChatMessageDto>.Failure("Authenticated user profile not found.");

        var conversation = await _unitOfWork.Conversations.GetByIdWithParticipantsAsync(request.ConversationId, ct);
        if (conversation == null)
            return Result<ChatMessageDto>.Failure("Conversation not found.");

        if (!conversation.Participants.Any(p => p.ProfileId == senderProfile.Id))
            return Result<ChatMessageDto>.Failure("You are not a participant in this conversation.");

        var senderType = senderProfile.Role == UserRole.Mechanic ? MessageSenderType.Mechanic : MessageSenderType.Client;

        var message = new BookNow.Domain.Entities.Message(
            request.ConversationId,
            senderProfile.Id,
            senderType,
            request.Content.Trim(),
            request.ImageUrl);

        await _unitOfWork.Messages.AddAsync(message, ct);
        conversation.Touch();
        await _unitOfWork.SaveChangesAsync(ct);

        var recipientIds = conversation.Participants
            .Select(p => p.Profile.IdentityUserId)
            .Distinct()
            .ToList();

        await _realTimeChatService.PushNewMessageAsync(recipientIds, new
        {
            conversationId = message.ConversationId,
            message = MapMessage(message),
            senderProfileId = senderProfile.Id,
            senderIdentityUserId = senderProfile.IdentityUserId,
            lastMessage = string.IsNullOrWhiteSpace(message.Content) ? "Image" : message.Content,
            lastMessageAt = message.CreatedAt
        }, ct);

        return Result<ChatMessageDto>.Success(MapMessage(message), "Message sent successfully.");
    }

    private static ChatMessageDto MapMessage(BookNow.Domain.Entities.Message message)
        => new()
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderProfileId = message.SenderProfileId,
            SenderType = message.SenderType,
            Content = message.Content,
            ImageUrl = message.ImageUrl,
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };
}
