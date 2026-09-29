using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetUnreadAsync(string identityUserId, CancellationToken cancellationToken);
    Task MarkAsReadAsync(string identityUserId, string type, CancellationToken cancellationToken);
    Task AddAsync(Notification notification, CancellationToken cancellationToken);
}