using BookNow.Application.Features.Workshop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Workshop.Handler.Commands;

public class VerifyWorkshopCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<VerifyWorkshopCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyWorkshopCommand request, CancellationToken cancellationToken)
    {
        var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, cancellationToken);
        if (workshop == null)
            return Result<string>.Failure("Workshop not found.");

        workshop.Verify();
        unitOfWork.Workshops.Update(workshop);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success($"Workshop {workshop.Name} has been verified.", "Success");
    }
}
