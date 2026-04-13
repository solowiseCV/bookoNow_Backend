using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Handler.Queries;

public sealed class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, Result<CursorPaginatedResult<ChatMessageDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationMessagesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CursorPaginatedResult<ChatMessageDto>>> Handle(GetConversationMessagesQuery request, CancellationToken ct)
    {
        if (request.ViewerIdentityUserId == Guid.Empty)
            return Result<CursorPaginatedResult<ChatMessageDto>>.Failure("Invalid authenticated user.");

        var viewerProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.ViewerIdentityUserId, ct);
        if (viewerProfile == null)
            return Result<CursorPaginatedResult<ChatMessageDto>>.Failure("User profile not found.");

        var isParticipant = await _unitOfWork.Conversations.IsParticipantAsync(request.ConversationId, viewerProfile.Id, ct);
        if (!isParticipant)
            return Result<CursorPaginatedResult<ChatMessageDto>>.Failure("You are not a participant in this conversation.");

        var (messages, nextCursor) = await _unitOfWork.Messages.GetMessagesAsync(request.ConversationId, request.Cursor, request.PageSize, ct);

        var dtos = messages.Select(m => new ChatMessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderProfileId = m.SenderProfileId,
            SenderType = m.SenderType,
            Content = m.Content,
            ImageUrl = m.ImageUrl,
            IsRead = m.IsRead,
            ReadAt = m.ReadAt,
            CreatedAt = m.CreatedAt
        }).ToList();

        return Result<CursorPaginatedResult<ChatMessageDto>>.Success(new CursorPaginatedResult<ChatMessageDto>(dtos, nextCursor), "Messages retrieved successfully.");
    }
}
