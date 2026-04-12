using BookNow.Application.DTOs.Chat;
using BookNow.Application.Models;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Chat.Request.Queries;

public sealed record GetUserConversationsQuery(
    Guid ViewerIdentityUserId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedResult<ChatConversationDto>>>;
