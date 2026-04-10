using BookNow.Application.Features.Workshop.Request.Commands;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Domain.Common;
using MediatR;

namespace BookNow.Application.Features.Workshop.Handler.Commands;

public class VerifyWorkshopCommandHandler(
    IUnitOfWork unitOfWork,
    IBackgroundJobService backgroundJobService) : IRequestHandler<VerifyWorkshopCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyWorkshopCommand request, CancellationToken cancellationToken)
    {
        var workshop = await unitOfWork.Workshops.GetByIdAsync(request.WorkshopId, cancellationToken);
        if (workshop == null)
            return Result<string>.Failure("Workshop not found.");

        workshop.Verify();
        unitOfWork.Workshops.Update(workshop);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify Mechanic via Background Job
        if (workshop.MechanicProfile != null)
        {
            backgroundJobService.Enqueue<INotificationService>(service => 
                service.SendNotificationAsync(workshop.MechanicProfile.IdentityUserId, 
                    workshop.MechanicProfile.PhoneNumber, 
                    $"Congratulations! Your workshop '{workshop.Name}' has been verified and is now live.", 
                    CancellationToken.None));

            backgroundJobService.Enqueue<IEmailService>(service => 
                service.SendNotificationEmailAsync(workshop.MechanicProfile.Email, 
                    "Workshop Verified", "Verification Successful", 
                    $"Great news! Your workshop '{workshop.Name}' has been verified by our team. Users can now discover and book services at your workshop.", 
                    "View Dashboard", "https://booknow-three.vercel.app/dashboard"));
        }

        return Result<string>.Success($"Workshop {workshop.Name} has been verified.", "Success");
    }
}
