using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Workshop.Request.Commands;

public class VerifyWorkshopCommand : IRequest<Result<string>>
{
    public Guid WorkshopId { get; set; }

    public VerifyWorkshopCommand(Guid workshopId)
    {
        WorkshopId = workshopId;
    }
}
