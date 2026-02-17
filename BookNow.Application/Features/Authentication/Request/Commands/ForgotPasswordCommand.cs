using BookNow.Application.DTOs.Authentication.Request;
using BookNow.Application.DTOs.Authentication.Response;
using MediatR;

namespace BookNow.Application.Features.Authentication.Request.Commands;

public record ForgotPasswordCommand(ForgotPasswordRequestDto ForgotPasswordRequest) : IRequest<AuthResultDto>;
