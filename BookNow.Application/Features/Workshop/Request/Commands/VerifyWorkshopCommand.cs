using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Workshop.Request.Commands;

public class VerifyWorkshopCommand(Guid workshopId) : IRequest<Result<string>>
{
    public Guid WorkshopId { get; set; } = workshopId;
}
