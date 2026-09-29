using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionInterventions.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetUnreadAsync(string identityUserId, CancellationToken cancellationToken)
    {
        return await _context.Notifications
            .Where(n => n.IdentityUserId == identityUserId && !n.EstLue)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(string identityUserId, string type, CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .Where(n => n.IdentityUserId == identityUserId && !n.EstLue && n.Type == type)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.MarquerCommeLue();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
    }
}