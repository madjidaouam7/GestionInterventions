namespace GestionInterventions.Web.Services;

public class ResponsableValidationNotificationStateService : IDisposable
{
    private readonly NotificationHubService _notificationHubService;
    private List<int> _interventionIds = new();

    public IReadOnlyList<int> InterventionIds => _interventionIds;

    public bool HasUnreadInterventionTerminee { get; private set; }

    public event Action? OnStateChanged;

    public ResponsableValidationNotificationStateService(NotificationHubService notificationHubService)
    {
        _notificationHubService = notificationHubService;
        _notificationHubService.NotificationInterventionTermineeReceived += HandleInterventionTerminee;
    }

    private void HandleInterventionTerminee(int interventionId, string message)
    {
        if (!_interventionIds.Contains(interventionId))
        {
            _interventionIds.Add(interventionId);
        }

        HasUnreadInterventionTerminee = true;
        NotifyStateChanged();
    }

    public void Dispose()
    {
        _notificationHubService.NotificationInterventionTermineeReceived -= HandleInterventionTerminee;
    }

    public void SetInterventionTermineeNotifications(
        IEnumerable<int> interventionIds)
    {
        _interventionIds = _interventionIds
            .Concat(interventionIds)
            .Distinct()
            .ToList();

        HasUnreadInterventionTerminee =
            _interventionIds.Count > 0;

        NotifyStateChanged();
    }

    public void MarkInterventionTermineeNotificationsAsRead()
    {
        HasUnreadInterventionTerminee = false;

        NotifyStateChanged();
    }

    public bool EstNouvelleIntervention(int interventionId)
    {
        return _interventionIds.Contains(interventionId);
    }

    public void RemoveInterventionNotification(int interventionId)
    {
        _interventionIds.Remove(interventionId);
        if (_interventionIds.Count == 0)
            HasUnreadInterventionTerminee = false;

        NotifyStateChanged();
    }

    public void ClearInterventionTermineeNotifications()
    {
        if (_interventionIds.Count == 0)
            return;

        _interventionIds.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}