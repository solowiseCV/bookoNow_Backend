using MediatR;

namespace BookNow.Application.Features.Workshops.Request.Commands.DeleteWorkshop;

public sealed record DeleteWorkshopCommand(Guid Id) : IRequest<Unit>;
