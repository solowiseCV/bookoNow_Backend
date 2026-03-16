namespace BookNow.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IUserProfileRepository UserProfiles { get; }
    IWorkshopRepository Workshops { get; }
    IAppointmentRepository Appointments { get; }
    IShopRepository Shops { get; }
    IProductRepository Products { get; }
    IReviewRepository Reviews { get; }
    IPaymentRepository Payments { get; }
    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
