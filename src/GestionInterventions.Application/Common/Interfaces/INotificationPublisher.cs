namespace GestionInterventions.Application.Common.Interfaces;

public interface INotificationPublisher
{
    Task PublierNouvelleDemandeAsync(
        int demandeId,
        string message,
        CancellationToken cancellationToken = default);

    Task PublierNouvelleInterventionAsync(
        int interventionId,
        string message,
        CancellationToken cancellationToken = default);

    Task PublierInterventionTermineeAsync(
        int interventionId,
        string message,
        CancellationToken cancellationToken = default);
}