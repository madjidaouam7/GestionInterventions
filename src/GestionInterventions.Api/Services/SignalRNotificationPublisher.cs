using GestionInterventions.Api.Hubs;
using GestionInterventions.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GestionInterventions.Api.Services;

public class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task PublierNouvelleDemandeAsync(
        int demandeId,
        string message,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(
            "NouvelleDemande",
            demandeId,
            message,
            cancellationToken);
    }

    public Task PublierNouvelleInterventionAsync(
        int interventionId,
        string message,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync(
            "NouvelleIntervention",
            interventionId,
            message,
            cancellationToken);
    }

    public Task PublierInterventionTermineeAsync(
        int interventionId,
        string message,
        CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.All.SendAsync(
            "InterventionTerminee",
            interventionId,
            message,
            cancellationToken);
    }
}