namespace GestionInterventions.Web.Services;

public class TechnicienNotificationStateService
{
    private bool _hasUnreadNouvelleIntervention;
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

    public void SetUnreadState(bool hasUnreadNouvelleIntervention)
    {
        if (hasUnreadNouvelleIntervention)
        {
            _hasUnconsultedAfaireNotification = true;
            AfaireFilterConsulted = false;
            HasUnreadNouvelleIntervention = true;
            return;
        }

        _hasUnconsultedAfaireNotification = false;
        HasUnreadNouvelleIntervention = false;
        AfaireFilterConsulted = true;
    }

    public void MarkNavbarNotificationsAsRead()
    {
        HasUnreadNouvelleIntervention = false;
    }

    public void SetNewInterventionIds(IEnumerable<int> interventionIds)
    {
        _newInterventionIds = interventionIds.Distinct().ToList();
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
        _hasUnconsultedAfaireNotification = false;
        AfaireFilterConsulted = true;
        HasUnreadNouvelleIntervention = false;
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();
}
