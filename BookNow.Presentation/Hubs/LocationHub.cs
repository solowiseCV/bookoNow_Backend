using BookNow.Application.Interfaces.Authentication;
using BookNow.Presentation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookNow.Presentation.Hubs;

/// <summary>
/// Real-time location hub.
/// Mechanics push their GPS coords; clients receive updates.
/// </summary>
[Authorize]
public sealed class LocationHub : Hub
{
    private readonly ILocationCache _cache;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<LocationHub> _logger;

    public LocationHub(
        ILocationCache cache,
        ICurrentUserService currentUser,
        ILogger<LocationHub> logger)
    {
        _cache = cache;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Called by the mechanic to broadcast their current GPS position.
    /// </summary>
    public async Task UpdateLocation(string appointmentId, double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;

        var userId = _currentUser.UserId ?? Context.UserIdentifier ?? "unknown";
        _logger.LogDebug("Mechanic {UserId} updated location for appointment {AppointmentId}: ({Lat},{Lng})",
            userId, appointmentId, latitude, longitude);

        var payload = new LocationPayload(latitude, longitude, DateTimeOffset.UtcNow);

        // Persist for late-joining clients
        await _cache.SetAsync(appointmentId, payload);

        // Broadcast to all subscribers of this appointment group
        var groupName = GroupName(appointmentId);
        await Clients.Group(groupName).SendAsync("LocationUpdated", payload);
    }

    /// <summary>
    /// Called by the client to start receiving location updates for an appointment.
    /// </summary>
    public async Task SubscribeToTracking(string appointmentId)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;

        var groupName = GroupName(appointmentId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Send the last known location immediately
        var last = await _cache.GetAsync(appointmentId);
        if (last is not null)
        {
            await Clients.Caller.SendAsync("LocationUpdated", last);
        }

        _logger.LogDebug("ConnectionId {Conn} subscribed to appointment {AppointmentId}",
            Context.ConnectionId, appointmentId);
    }

    /// <summary>
    /// Called when the mechanic stops sharing, or client unsubscribes.
    /// </summary>
    public async Task UnsubscribeFromTracking(string appointmentId)
    {
        if (string.IsNullOrWhiteSpace(appointmentId)) return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(appointmentId));
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static string GroupName(string appointmentId) => $"appointment-{appointmentId}";
}

/// <summary>DTO sent over the wire.</summary>
public sealed record LocationPayload(double Latitude, double Longitude, DateTimeOffset UpdatedAt);
