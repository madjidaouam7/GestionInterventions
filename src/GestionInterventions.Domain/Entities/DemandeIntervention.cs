using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Entities;

public class DemandeIntervention
{
    public int Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime DateDemande { get; private set; }

    public int EquipementId { get; private set; }
    public Equipement Equipement  { get; private set; } = null!;

    public StatutDemande Statut { get; private set; } = StatutDemande.EnAttente;
    public PrioriteDemande Priorite { get; private set; }

    public ICollection<Intervention> Interventions { get; private set; } = new List<Intervention>();

    private DemandeIntervention() { } // Pour EF Core

    public DemandeIntervention(string description, int equipementId, PrioriteDemande priorite)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La description du probleme est obligatoire.");

        if (!Enum.IsDefined(priorite))
            throw new DomainException("Le niveau de priorité est invalide (normale ou urgente).");

        if (equipementId <= 0)
            throw new DomainException("L'Id de l'equipement est obligatoire.");

        Description = description;
        EquipementId = equipementId;
        Priorite = priorite;
        DateDemande = DateTime.UtcNow;
    }




    public void Accepter()
    {
        if (Statut != StatutDemande.EnAttente)
            throw new DomainException("Une demande ne peut etre accepté que si elle est en attente.");

        Statut = StatutDemande.Acceptee;
    }


    public void Refuser()
    {
        if (Statut != StatutDemande.EnAttente)
            throw new DomainException("Une demande ne peut etre refusé que si elle est en attente.");
        Statut = StatutDemande.Refusee;
    }

}