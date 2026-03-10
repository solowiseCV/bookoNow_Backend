using BookNow.Application.DTOs.Authentication.Response;
using MediatR;

namespace BookNow.Application.Features.Authentication.Request.Queries;

public record GetProfileQuery(string UserId) : IRequest<AuthResultDto>;
