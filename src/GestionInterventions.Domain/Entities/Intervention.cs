using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Entities;

public class Intervention
{
    public int Id { get; private set; }
    public DateTime DatePrevue { get; private set; }

    public int DemandeId { get; private set; }
    public DemandeIntervention Demande { get; private set; } = null!;

    public int TechnicienId { get; private set; }
    public Technicien Technicien { get; private set; } = null!;

    public StatutIntervention Statut { get; private set; } = StatutIntervention.Planifiee;
    public CompteRendu? CompteRendu { get; private set; }

    private Intervention() { } // Pour EF Core

    public Intervention(int demandeId, int technicienId, DateTime datePrevue)
    {
        if (datePrevue <= DateTime.UtcNow)
            throw new DomainException("La date prévue de l'intervention doit être ultérieure à la date actuelle.");

        if (demandeId <= 0)
            throw new DomainException("L'Id de la demande est obligatoire.");

        if (technicienId <= 0)
            throw new DomainException("L'Id du technicien est obligatoire.");

        DemandeId = demandeId;
        TechnicienId = technicienId;
        DatePrevue = datePrevue;
    }


    public void ModifierDatePrevue(DateTime nouvelleDatePrevue)
    {
        if (Statut != StatutIntervention.Planifiee)
            throw new DomainException("Impossible de modifier la planification d'une intervention qui a déjà commencé ou terminé.");


        if (nouvelleDatePrevue <= DateTime.UtcNow)
            throw new DomainException("La date prévue de l'intervention doit être ultérieure à la date actuelle.");

        DatePrevue = nouvelleDatePrevue;
    }


    public void Commencer()
    {
        if (Statut != StatutIntervention.Planifiee)
            throw new DomainException("Une intervention ne peut pas commencer si elle n'est pas planifiée.");

        Statut = StatutIntervention.EnCours;
    }

    public void Terminer(CompteRendu compteRendu)
    {
        if (Statut != StatutIntervention.EnCours)
            throw new DomainException("Une intervention ne peut se terminer que si elle est en cours.");
        CompteRendu = compteRendu;
        Statut = StatutIntervention.Terminee;
    }

    public void Valider()
    {
        if (Statut != StatutIntervention.Terminee)
            throw new DomainException("Une intervention ne peut etre validée que si elle est terminée.");
        Statut = StatutIntervention.Cloturee;
    }

    public void DemanderCorrection()
    {
        if (Statut != StatutIntervention.Terminee)
            throw new DomainException("Une correction ne peut être demandée que pour une intervention déjà terminée.");
        Statut = StatutIntervention.EnCours;
    }

}