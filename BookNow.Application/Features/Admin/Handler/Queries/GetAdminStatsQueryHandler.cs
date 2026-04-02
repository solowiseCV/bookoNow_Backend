using BookNow.Application.DTOs.Admin;
using BookNow.Application.Interfaces.Persistence;
using BookNow.Application.Features.Admin.Queries;
using MediatR;
using BookNow.Domain.Common;
using BookNow.Domain.Enums;

namespace BookNow.Application.Features.Admin.Handler.Queries;

public class GetAdminStatsQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetAdminStatsQuery, Result<AdminStatsDto>>
{
    public async Task<Result<AdminStatsDto>> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var totalUsers = await unitOfWork.UserProfiles.CountAsync(cancellationToken);
        
        var shops = await unitOfWork.Shops.GetAllAsync(cancellationToken);
        var totalShops = shops.Count();
        var pendingShops = shops.Count(s => s.Status == ShopStatus.Pending);

        var workshopsQuery = await unitOfWork.Workshops.GetAllAsync(cancellationToken);
        var totalWorkshops = workshopsQuery.Count();
        var pendingWorkshops = workshopsQuery.Count(w => !w.IsVerified);

        var totalProducts = await unitOfWork.Products.CountAsync(cancellationToken);
        var totalAppointments = await unitOfWork.Appointments.CountAsync(cancellationToken);

        var stats = new AdminStatsDto(
            totalUsers,
            totalShops,
            pendingShops,
            totalWorkshops,
            pendingWorkshops,
            totalProducts,
            totalAppointments
        );

        return Result<AdminStatsDto>.Success(stats);
    }
}
