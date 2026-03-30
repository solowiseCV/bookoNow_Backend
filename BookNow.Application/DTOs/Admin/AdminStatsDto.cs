namespace BookNow.Application.DTOs.Admin;

public sealed record AdminStatsDto(
    int TotalUsers,
    int TotalShops,
    int PendingShops,
    int TotalWorkshops,
    int PendingWorkshops,
    int TotalProducts,
    int TotalAppointments
);
