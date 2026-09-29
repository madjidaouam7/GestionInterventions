using GestionInterventions.Domain.Exceptions;

public class Notification
{
    public int Id { get; private set; }

    public string IdentityUserId { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public DateTime DateCreation { get; private set; }

    public bool EstLue { get; private set; }

    public int? DemandeId { get; private set; }

    public int? InterventionId { get; private set; }

    private Notification() { }

    public Notification(
        string identityUserId,
        string type,
        string message,
        int? demandeId = null,
        int? interventionId = null)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new DomainException("L'Id de l'utilisateur est obligatoire.");

        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Le type de notification est obligatoire.");

        if (string.IsNullOrWhiteSpace(message))
            throw new DomainException("Le message de notification est obligatoire.");

        IdentityUserId = identityUserId;
        Type = type;
        Message = message;
        DateCreation = DateTime.UtcNow;
        EstLue = false;
        DemandeId = demandeId;
        InterventionId = interventionId;
    }

    public void MarquerCommeLue()
    {
        EstLue = true;
    }
}