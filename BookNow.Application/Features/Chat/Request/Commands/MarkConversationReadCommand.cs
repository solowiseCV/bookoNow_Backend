using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Request.Commands;

public sealed record MarkConversationReadCommand(Guid ConversationId, Guid ViewerIdentityUserId) : IRequest<Result<bool>>;
