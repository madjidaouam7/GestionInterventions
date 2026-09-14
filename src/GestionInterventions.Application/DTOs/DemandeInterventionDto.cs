using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.DTOs;

public class DemandeInterventionDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateDemande { get; set; }
    public int EquipementId { get; set; }
    public string EquipementNom { get; set; } = string.Empty;
    public PrioriteDemande Priorite { get; set; }
    public StatutDemande Statut { get; set; }
    public int? InterventionId { get; set; }
    public DateTime? DatePrevue { get; set; }
    public string? InterventionStatut { get; set; }
    public DemandeInterventionDto() { }
    public DemandeInterventionDto(DemandeIntervention demandeIntervention)
    {
        Id = demandeIntervention.Id;
        Description = demandeIntervention.Description;
        DateDemande = demandeIntervention.DateDemande;
        EquipementId = demandeIntervention.EquipementId;
        EquipementNom = demandeIntervention.Equipement?.Nom ?? string.Empty;
        Priorite = demandeIntervention.Priorite;
        Statut = demandeIntervention.Statut;
    }

}