using BookNow.Application.Interfaces.Persistence;
using BookNow.Infrastructure.Data;

namespace BookNow.Infrastructure.Repositories;

public sealed class UnitOfWork(
    BookNowDbContext context,
    IUserProfileRepository userProfiles,
    IWorkshopRepository workshops,
    IAppointmentRepository appointments
) : IUnitOfWork
{
    public IUserProfileRepository UserProfiles { get; } = userProfiles;
    public IWorkshopRepository Workshops { get; } = workshops;
    public IAppointmentRepository Appointments { get; } = appointments;

    public async Task<int> SaveChangesAsync(CancellationToken ct) => await context.SaveChangesAsync(ct);

}
