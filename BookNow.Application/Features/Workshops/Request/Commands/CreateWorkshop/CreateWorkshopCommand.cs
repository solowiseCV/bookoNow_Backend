
using MediatR;
namespace BookNow.Application.Features.Workshops.Request.Commands.CreateWorkshop;


public sealed record CreateWorkshopCommand(
    string Name,
    string Description,
    string Address,
    double Latitude,
    double Longitude
) : IRequest<Guid>;


