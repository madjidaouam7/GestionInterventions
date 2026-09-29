namespace GestionInterventions.Application.Features.Notifications;

public class NotificationDto
{
    public int Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime DateCreation { get; set; }

    public bool EstLue { get; set; }

    public int? DemandeId { get; set; }

    public int? InterventionId { get; set; }

    public NotificationDto() { }

    public NotificationDto(Notification notification)
    {
        Id = notification.Id;
        Type = notification.Type;
        Message = notification.Message;
        DateCreation = notification.DateCreation;
        EstLue = notification.EstLue;
        DemandeId = notification.DemandeId;
        InterventionId = notification.InterventionId;
    }
}