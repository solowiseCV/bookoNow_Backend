using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;
using MediatR;

namespace BookNow.Application.Features.Chat.Handler.Commands;

public sealed class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ChatConversationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ChatConversationDto>> Handle(CreateConversationCommand request, CancellationToken ct)
    {
        if (request.RequesterIdentityUserId == Guid.Empty)
            return Result<ChatConversationDto>.Failure("Invalid authenticated user.");

        if (request.TargetProfileId == Guid.Empty)
            return Result<ChatConversationDto>.Failure("Target profile id is required.");

        var requesterProfile = await _unitOfWork.UserProfiles.GetByIdentityIdAsync(request.RequesterIdentityUserId, ct);
        if (requesterProfile == null)
            return Result<ChatConversationDto>.Failure("Authenticated user profile not found.");

        var existingTargetProfile = await _unitOfWork.UserProfiles.GetByIdAsync(request.TargetProfileId, ct);
        if (existingTargetProfile == null)
            return Result<ChatConversationDto>.Failure("Target profile not found.");

        if (requesterProfile.Id == existingTargetProfile.Id)
            return Result<ChatConversationDto>.Failure("Cannot create a conversation with yourself.");

        var existingConversation = await _unitOfWork.Conversations.GetOneOnOneConversationAsync(requesterProfile.Id, existingTargetProfile.Id, ct);
        if (existingConversation != null)
            return Result<ChatConversationDto>.Success(MapConversation(existingConversation), "Conversation already exists.");

        var conversation = new BookNow.Domain.Entities.Conversation(request.AppointmentId);
        conversation.AddParticipant(requesterProfile.Id);
        conversation.AddParticipant(existingTargetProfile.Id);

        await _unitOfWork.Conversations.AddAsync(conversation, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<ChatConversationDto>.Success(MapConversation(conversation), "Conversation created successfully.");
    }

    private static ChatConversationDto MapConversation(BookNow.Domain.Entities.Conversation conversation)
        => new()
        {
            Id = conversation.Id,
            AppointmentId = conversation.AppointmentId,
            ParticipantIds = conversation.Participants.Select(p => p.ProfileId).ToList(),
            LastMessage = conversation.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Content,
            LastMessageAt = conversation.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.CreatedAt
        };
}
