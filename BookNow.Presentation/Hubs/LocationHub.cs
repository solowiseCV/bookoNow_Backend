using BookNow.Application.Interfaces.Authentication;
using BookNow.Presentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookNow.Presentation.Hubs;


[Authorize]
public sealed class LocationHub(
    ILocationCache cache,
    ICurrentUserService currentUser,
    ILogger<LocationHub> logger) : Hub
{
    public async Task UpdateLocation(string appointmentId, double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;

        var userId = currentUser.UserId ?? Context.UserIdentifier ?? "unknown";
        logger.LogDebug("Mechanic {UserId} updated location for appointment {AppointmentId}: ({Lat},{Lng})",
            userId, appointmentId, latitude, longitude);

        var payload = new LocationPayload(latitude, longitude, DateTimeOffset.UtcNow);

        // Persist for late-joining clients
        await cache.SetAsync(appointmentId, payload);

        // Broadcast to all subscribers of this appointment group
        var groupName = GroupName(appointmentId);
        await Clients.Group(groupName).SendAsync("LocationUpdated", payload);
    }
    public async Task SubscribeToTracking(string appointmentId)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;

        var groupName = GroupName(appointmentId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Send the last known location immediately
        var last = await cache.GetAsync(appointmentId);
        if (last is not null)
        {
            await Clients.Caller.SendAsync("LocationUpdated", last);
        }

        logger.LogDebug("ConnectionId {Conn} subscribed to appointment {AppointmentId}",
            Context.ConnectionId, appointmentId);
    }
    public async Task UnsubscribeFromTracking(string appointmentId)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(appointmentId));
    }

    public async Task StopSharing(string appointmentId)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;

        var groupName = GroupName(appointmentId);
        await Clients.Group(groupName).SendAsync("LocationSharingStopped");

        logger.LogDebug("Mechanic {UserId} stopped sharing for appointment {AppointmentId}",
            currentUser.UserId ?? Context.UserIdentifier ?? "unknown", appointmentId);
    }

     private static string GroupName(string appointmentId) => $"appointment-{appointmentId}";
}
public sealed record LocationPayload(double Latitude, double Longitude, DateTimeOffset UpdatedAt);
