using BookNow.Application.Interfaces.Persistence;
using BookNow.Infrastructure.Data;

namespace BookNow.Infrastructure.Repositories;

public sealed class UnitOfWork(
    BookNowDbContext context,
    IUserProfileRepository userProfiles,
    IWorkshopRepository workshops,
    IAppointmentRepository appointments,
    IShopRepository shops,
    IProductRepository products,
    IReviewRepository reviews,
    IPaymentRepository payments,
    IOrderRepository orders
) : IUnitOfWork
{
    public IUserProfileRepository UserProfiles { get; } = userProfiles;
    public IWorkshopRepository Workshops { get; } = workshops;
    public IAppointmentRepository Appointments { get; } = appointments;
    public IShopRepository Shops { get; } = shops;
    public IProductRepository Products { get; } = products;
    public IReviewRepository Reviews { get; } = reviews;
    public IPaymentRepository Payments { get; } = payments;
    public IOrderRepository Orders { get; } = orders;

    public async Task<int> SaveChangesAsync(CancellationToken ct) => await context.SaveChangesAsync(ct);

}
