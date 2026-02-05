namespace BookNow.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IUserProfileRepository UserProfiles { get; }
    IWorkshopRepository Workshops { get; }
    IAppointmentRepository Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
