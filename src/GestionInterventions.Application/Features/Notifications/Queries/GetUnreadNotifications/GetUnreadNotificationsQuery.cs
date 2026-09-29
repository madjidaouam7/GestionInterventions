using MediatR;

namespace GestionInterventions.Application.Features.Notifications.Queries.GetUnreadNotifications;

public record GetUnreadNotificationsQuery(string IdentityUserId) : IRequest<List<NotificationDto>>;