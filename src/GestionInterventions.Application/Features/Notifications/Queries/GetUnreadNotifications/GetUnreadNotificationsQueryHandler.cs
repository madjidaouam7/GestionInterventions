using GestionInterventions.Application.Common.Interfaces;
using MediatR;

namespace GestionInterventions.Application.Features.Notifications.Queries.GetUnreadNotifications;

public class GetUnreadNotificationsQueryHandler : IRequestHandler<GetUnreadNotificationsQuery, List<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetUnreadAsync(request.IdentityUserId, cancellationToken);

        return notifications.Select(n => new NotificationDto(n)).ToList();
    }
}