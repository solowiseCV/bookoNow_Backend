using BookNow.Application.DTOs.Admin;
using BookNow.Application.Common;
using MediatR;
using BookNow.Domain.Common;

namespace BookNow.Application.Features.Admin.Queries;

public record GetAdminStatsQuery() : IRequest<Result<AdminStatsDto>>;
