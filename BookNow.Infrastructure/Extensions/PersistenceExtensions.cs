using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Interfaces.Services;
using BookNow.Infrastructure.BackgroundJobs;
using BookNow.Infrastructure.Repositories;
using BookNow.Infrastructure.Services;
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
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IWorkshopReadRepository, WorkshopReadRepository>();
        
        services.AddHostedService<RevokedTokenCleanupService>();
        
        // External services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IMediaStorageService, MediaStorageService>();
        return services;
    }
}
