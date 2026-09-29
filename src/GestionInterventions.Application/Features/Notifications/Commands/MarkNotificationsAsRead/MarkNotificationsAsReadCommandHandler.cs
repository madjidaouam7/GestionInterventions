using GestionInterventions.Application.Common.Interfaces;
using MediatR;

namespace GestionInterventions.Application.Features.Notifications.Commands.MarkNotificationsAsRead;

public class MarkNotificationsAsReadCommandHandler : IRequestHandler<MarkNotificationsAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationsAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(MarkNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await _notificationRepository.MarkAsReadAsync(request.IdentityUserId, request.Type, cancellationToken);
    }
}