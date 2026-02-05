using BookNow.Application.Interfaces.Persistence;
using BookNow.Infrastructure.Data;


namespace BookNow.Infrastructure.Repositories;
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly BookNowDbContext _context;

    public IUserProfileRepository UserProfiles { get; }
    public IWorkshopRepository Workshops { get; }
    public IAppointmentRepository Appointments { get; }

    public UnitOfWork(
        BookNowDbContext context,
        IUserProfileRepository userProfiles,
        IWorkshopRepository workshops,
        IAppointmentRepository appointments)
    {
        _context = context;

        UserProfiles = userProfiles;
        Workshops = workshops;
        Appointments = appointments;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct) => await _context.SaveChangesAsync(ct);

}

