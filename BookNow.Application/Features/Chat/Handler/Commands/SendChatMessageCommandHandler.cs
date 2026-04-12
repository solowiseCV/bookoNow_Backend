using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Chat.Handler.Commands;

public sealed class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Result<ChatMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendChatMessageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChatMessageDto>> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        if (request.SenderIdentityUserId == Guid.Empty)
            return Result<ChatMessageDto>.Failure("Invalid authenticated user.");

        if (request.ConversationId == Guid.Empty)
            return Result<ChatMessageDto>.Failure("Conversation id is required.");

        if (string.IsNullOrWhiteSpace(request.Content))
            return Result<ChatMessageDto>.Failure("Message content is required.");

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
            request.Content.Trim());

        await _unitOfWork.Messages.AddAsync(message, ct);
        conversation.Touch();
        await _unitOfWork.SaveChangesAsync(ct);

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
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };
}
