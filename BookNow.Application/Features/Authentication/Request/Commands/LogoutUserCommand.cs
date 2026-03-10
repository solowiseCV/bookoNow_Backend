using BookNow.Application.DTOs.Authentication.Response;
using MediatR;

namespace BookNow.Application.Features.Authentication.Request.Commands;

public record LogoutUserCommand() : IRequest<AuthResultDto>;
