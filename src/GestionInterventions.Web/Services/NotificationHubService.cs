using Microsoft.AspNetCore.SignalR.Client;

namespace GestionInterventions.Web.Services;

public class NotificationHubService : IAsyncDisposable
{
    private readonly HubConnection _connection;

    public bool IsConnected =>
        _connection.State == HubConnectionState.Connected;

    public event Action<int, string>? NotificationNouvelleDemandeReceived;
    public event Action<int, string>? NotificationNouvelleInterventionReceived;
    public event Action<int, string>? NotificationInterventionTermineeReceived;

    public NotificationHubService()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5084/hubs/notifications")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<int, string>("NouvelleDemande", (demandeId, message) =>
            NotificationNouvelleDemandeReceived?.Invoke(demandeId, message));

        _connection.On<int, string>("NouvelleIntervention", (interventionId, message) =>
            NotificationNouvelleInterventionReceived?.Invoke(interventionId, message));

        _connection.On<int, string>("InterventionTerminee", (interventionId, message) =>
            NotificationInterventionTermineeReceived?.Invoke(interventionId, message));
    }

    public async Task StartAsync()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}