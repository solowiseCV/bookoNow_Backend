using BookNow.Application.DTOs.Chat;
using BookNow.Application.Features.Chat.Request.Commands;
using BookNow.Application.Features.Chat.Request.Queries;
using BookNow.Application.Models;
using BookNow.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BookNow.Presentation.Controllers;

[ApiController]
[Route("chat")]
[Authorize]
public class ChatController(ISender _sender) : BaseApiController
{
    [SwaggerOperation(Summary = "Retrieves the authenticated user's conversations.")]
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ChatConversationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _sender.Send(new GetUserConversationsQuery(UserId, pageNumber, pageSize));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Creates a one-on-one conversation with another user.")]
    [HttpPost("conversations")]
    [ProducesResponseType(typeof(ApiResponse<ChatConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        var result = await _sender.Send(new CreateConversationCommand(UserId, request.TargetProfileId, request.AppointmentId));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Retrieves messages for a conversation with cursor pagination.")]
    [HttpGet("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<CursorPaginatedResult<ChatMessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversationMessages(Guid conversationId, [FromQuery] int pageSize = 50, [FromQuery] string? cursor = null)
    {
        var result = await _sender.Send(new GetConversationMessagesQuery(conversationId, UserId, pageSize, cursor));
        return HandleResult(result);
    }

    [SwaggerOperation(Summary = "Sends a message into an existing conversation.")]
    [HttpPost("conversations/{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<ChatMessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendChatMessageRequest request)
    {
        var result = await _sender.Send(new SendChatMessageCommand(conversationId, UserId, request.Content));
        return HandleResult(result);
    }

    public class CreateConversationRequest
    {
        public Guid TargetProfileId { get; set; }
        public Guid? AppointmentId { get; set; }
    }

    public class SendChatMessageRequest
    {
        public string Content { get; set; } = string.Empty;
    }
}
