using BookNow.Application.DTOs.Chat;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Request.Commands;

public sealed record SendChatMessageCommand(
    Guid ConversationId,
    Guid SenderIdentityUserId,
    string Content,
    string? ImageUrl = null
) : IRequest<Result<ChatMessageDto>>;
