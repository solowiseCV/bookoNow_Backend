using BookNow.Application.Interfaces.Persistence;
using BookNow.Infrastructure.Persistence;
using BookNow.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BookNow.Infrastructure.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IWorkshopRepository, WorkshopRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWorkshopReadRepository, WorkshopReadRepository>();
        return services;
    }
}
