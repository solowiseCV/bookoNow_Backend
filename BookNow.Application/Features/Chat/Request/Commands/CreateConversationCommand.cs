using BookNow.Application.DTOs.Chat;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Request.Commands;

public sealed record CreateConversationCommand(
    Guid RequesterIdentityUserId,
    Guid TargetProfileId,
    Guid? AppointmentId = null
) : IRequest<Result<ChatConversationDto>>;
