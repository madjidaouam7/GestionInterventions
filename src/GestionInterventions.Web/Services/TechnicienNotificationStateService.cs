namespace GestionInterventions.Web.Services;

public class TechnicienNotificationStateService : IDisposable
{
    private readonly NotificationHubService _notificationHubService;
    private bool _hasUnreadNouvelleIntervention;
    private bool _hasLiveNouvelleIntervention;
    private bool _afaireFilterConsulted;
    private bool _hasUnconsultedAfaireNotification;
    private List<int> _newInterventionIds = new();

    public bool HasUnreadNouvelleIntervention
    {
        get => _hasUnreadNouvelleIntervention;
        set
        {
            if (_hasUnreadNouvelleIntervention == value)
                return;

            _hasUnreadNouvelleIntervention = value;
            NotifyStateChanged();
        }
    }

    public bool AfaireFilterConsulted
    {
        get => _afaireFilterConsulted;
        set
        {
            if (_afaireFilterConsulted == value)
                return;

            _afaireFilterConsulted = value;
            NotifyStateChanged();
        }
    }

    public bool ShowAfaireNotificationBadge => _hasUnconsultedAfaireNotification && !AfaireFilterConsulted;

    public IReadOnlyList<int> NewInterventionIds => _newInterventionIds;

    public event Action? OnStateChanged;

    public TechnicienNotificationStateService(NotificationHubService notificationHubService)
    {
        _notificationHubService = notificationHubService;
        _notificationHubService.NotificationNouvelleInterventionReceived += HandleNouvelleIntervention;
    }

    private void HandleNouvelleIntervention(int interventionId, string message)
    {
        _hasLiveNouvelleIntervention = true;
        _newInterventionIds = _newInterventionIds.Append(interventionId).Distinct().ToList();
        SetUnreadState(true);
    }

    public void Dispose()
    {
        _notificationHubService.NotificationNouvelleInterventionReceived -= HandleNouvelleIntervention;
    }

    public void SetUnreadState(bool hasUnreadNouvelleIntervention)
    {
        if (hasUnreadNouvelleIntervention)
        {
            _hasUnconsultedAfaireNotification = true;
            AfaireFilterConsulted = false;
            HasUnreadNouvelleIntervention = true;
            return;
        }

        if (!_hasLiveNouvelleIntervention)
        {
            _hasUnconsultedAfaireNotification = false;
            HasUnreadNouvelleIntervention = false;
            AfaireFilterConsulted = true;
        }
    }

    public void MarkNavbarNotificationsAsRead()
    {
        _hasLiveNouvelleIntervention = false;
        HasUnreadNouvelleIntervention = false;
    }

    public void SetNewInterventionIds(IEnumerable<int> interventionIds)
    {
        _newInterventionIds = _newInterventionIds
            .Concat(interventionIds)
            .Distinct()
            .ToList();
        NotifyStateChanged();
    }

    public void ClearNewInterventionIds()
    {
        if (_newInterventionIds.Count == 0)
            return;

        _newInterventionIds.Clear();
        NotifyStateChanged();
    }

    public void MarkAfaireNotificationsAsRead()
    {
        _hasLiveNouvelleIntervention = false;
        _hasUnconsultedAfaireNotification = false;
        AfaireFilterConsulted = true;
        HasUnreadNouvelleIntervention = false;
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
