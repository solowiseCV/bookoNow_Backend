using BookNow.Application.DTOs.Chat;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Request.Queries;

public sealed record GetConversationMessagesQuery(
    Guid ConversationId,
    Guid ViewerIdentityUserId,
    int PageSize = 50,
    string? Cursor = null
) : IRequest<Result<CursorPaginatedResult<ChatMessageDto>>>;
