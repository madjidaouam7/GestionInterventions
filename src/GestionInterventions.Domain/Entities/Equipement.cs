using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Entities;

public class Equipement
{
    public int Id { get; private set; }
    public string Nom { get; private set; } = string.Empty;
    public string NumeroSerie { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Localisation { get; private set; } = string.Empty;

    public int ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public StatutEquipement Statut { get; private set; } = StatutEquipement.Fonctionnel;

    public ICollection<DemandeIntervention> Demandes { get; private set; } = new List<DemandeIntervention>();

    private Equipement() { } // Pour EF Core

    public Equipement(string nom, string numeroSerie, string description, string localisation, int clientId)
    {
        if (string.IsNullOrWhiteSpace(nom))
            throw new DomainException("Le nom de l'equipement est obligatoire.");

        if (string.IsNullOrWhiteSpace(numeroSerie))
            throw new DomainException("Le numero de serie de l'equipement est invalide.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La description de l'equipement est obligatoire.");

        if (string.IsNullOrWhiteSpace(localisation))
            throw new DomainException("La localisation de l'equipement est obligatoire.");

        if (clientId <= 0)
            throw new DomainException("L'Id du client est obligatoire.");

        Nom = nom;
        NumeroSerie = numeroSerie;
        Description = description;
        Localisation = localisation;
        ClientId = clientId;
    }




    public void SignalerPanne()
    {
        if (Statut != StatutEquipement.Fonctionnel)
            throw new DomainException("Impossible de signaler une panne pendant une maintenance en cours.");

        Statut = StatutEquipement.EnPanne;
    }


    public void DemarrerMaintenance()
    {
        if (Statut != StatutEquipement.EnPanne)
            throw new DomainException("Une maintenance ne peut être démarrée que pour un équipement en panne.");
        Statut = StatutEquipement.EnMaintenance;
    }


    public void RemettreEnService()
    {
        if (Statut != StatutEquipement.EnMaintenance)
            throw new DomainException("Un équipement doit être en maintenance avant d'être remis en service.");
        Statut = StatutEquipement.Fonctionnel;
    }


    public void SignalerEchecMaintenance()
    {
        if (Statut != StatutEquipement.EnMaintenance)
            throw new DomainException("Un échec de maintenance ne peut être signalé que pendant une maintenance.");
        Statut = StatutEquipement.EnPanne;
    }

    public void AnnulerSignalementPanne()
    {
        if (Statut != StatutEquipement.EnPanne)
            throw new DomainException("Impossible d'annuler un signalement pour un équipement qui n'est pas en panne.");

        Statut = StatutEquipement.Fonctionnel;
    }
}