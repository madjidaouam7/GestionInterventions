namespace GestionInterventions.Web.Services;

public class ResponsableNotificationStateService : IDisposable
{
    private readonly NotificationHubService _notificationHubService;

    private List<int> _nouvelleDemandeIds = new();

    public IReadOnlyList<int> NouvelleDemandeIds => _nouvelleDemandeIds;

    public bool HasUnreadNouvelleDemande { get; private set; }

    public event Action? OnStateChanged;

    public ResponsableNotificationStateService(NotificationHubService notificationHubService)
    {
        _notificationHubService = notificationHubService;

        _notificationHubService.NotificationNouvelleDemandeReceived += HandleNouvelleDemande;
    }

    private void HandleNouvelleDemande(int demandeId, string message)
    {
        if (!_nouvelleDemandeIds.Contains(demandeId))
        {
            _nouvelleDemandeIds.Add(demandeId);
        }

        HasUnreadNouvelleDemande = true;

        NotifyStateChanged();
    }

    public void Dispose()
    {
        _notificationHubService.NotificationNouvelleDemandeReceived -= HandleNouvelleDemande;
    }

    public void SetNouvelleDemandeNotifications(
        IEnumerable<int> demandeIds)
    {
        _nouvelleDemandeIds = _nouvelleDemandeIds
            .Concat(demandeIds)
            .Distinct()
            .ToList();

        HasUnreadNouvelleDemande = _nouvelleDemandeIds.Count > 0;

        NotifyStateChanged();
    }

    public void MarkNouvelleDemandeNotificationsAsRead()
    {
        HasUnreadNouvelleDemande = false;

        NotifyStateChanged();
    }

    public bool EstNouvelleDemande(int demandeId)
    {
        return _nouvelleDemandeIds.Contains(demandeId);
    }

    public void RemoveNouvelleDemandeNotification(int demandeId)
    {
        _nouvelleDemandeIds.Remove(demandeId);

        NotifyStateChanged();
    }

    public void ClearNouvelleDemandeNotifications()
    {
        if (_nouvelleDemandeIds.Count == 0)
            return;

        _nouvelleDemandeIds.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}