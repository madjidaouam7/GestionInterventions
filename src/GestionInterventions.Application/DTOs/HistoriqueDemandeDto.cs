using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

public class HistoriqueDemandeDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime DateDemande { get; set; }
    public PrioriteDemande Priorite { get; set; }
    public StatutDemande Statut { get; set; }

    public List<InterventionDto> Interventions { get; set; } = new();
    public HistoriqueDemandeDto(){}
    public HistoriqueDemandeDto(DemandeIntervention demande)
    {
        Id = demande.Id;
        Description = demande.Description;
        DateDemande = demande.DateDemande;
        Priorite = demande.Priorite;
        Statut = demande.Statut;

        foreach (var intervention in demande.Interventions)
        {
            var interventionDto = new InterventionDto(intervention);
            Interventions.Add(interventionDto);
        }
    }
}