using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Queries;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Handler.Queries;

public sealed class GetUserConversationsQueryHandler : IRequestHandler<GetUserConversationsQuery, Result<PaginatedResult<ChatConversationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserConversationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedResult<ChatConversationDto>>> Handle(GetUserConversationsQuery request, CancellationToken ct)
    {
        if (request.ViewerIdentityUserId == Guid.Empty)
            return Result<PaginatedResult<ChatConversationDto>>.Failure("Invalid authenticated user.");

        var viewerProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.ViewerIdentityUserId, ct);
        if (viewerProfile == null)
            return Result<PaginatedResult<ChatConversationDto>>.Failure("User profile not found.");

        var totalCount = await _unitOfWork.Conversations.CountByParticipantAsync(viewerProfile.Id, ct);
        var conversations = await _unitOfWork.Conversations.GetByParticipantAsync(viewerProfile.Id, request.PageNumber, request.PageSize, ct);

        var dtos = conversations.Select(c =>
        {
            var lastMessage = c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            var otherParticipant = c.Participants
                .FirstOrDefault(p => p.ProfileId != viewerProfile.Id);

            var displayName = otherParticipant?.Profile != null
                ? otherParticipant.Profile.Workshops.FirstOrDefault()?.Name ?? otherParticipant.Profile.FullName
                : "Conversation";

            var unreadCount = c.Messages
                .Count(m => !m.IsRead && m.SenderProfileId != viewerProfile.Id);

            return new ChatConversationDto
            {
                Id = c.Id,
                AppointmentId = c.AppointmentId,
                ParticipantIds = c.Participants.Select(p => p.ProfileId).ToList(),
                DisplayName = displayName,
                OtherParticipantId = otherParticipant?.ProfileId,
                UnreadCount = unreadCount,
                LastMessage = lastMessage?.Content,
                LastMessageAt = lastMessage?.CreatedAt
            };
        }).ToList();

        var paginatedResult = new PaginatedResult<ChatConversationDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        return Result<PaginatedResult<ChatConversationDto>>.Success(paginatedResult, "Conversations retrieved successfully.");
    }
}
