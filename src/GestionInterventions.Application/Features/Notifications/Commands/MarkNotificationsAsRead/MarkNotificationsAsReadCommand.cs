using MediatR;

namespace GestionInterventions.Application.Features.Notifications.Commands.MarkNotificationsAsRead;

public record MarkNotificationsAsReadCommand(string IdentityUserId, string Type) : IRequest;