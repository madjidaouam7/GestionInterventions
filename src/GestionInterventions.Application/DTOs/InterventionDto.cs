using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.DTOs;

public class InterventionDto
{
    public int Id { get; set; }
    public DateTime DatePrevue { get; set; }
    public int DemandeId { get; set; }
    public int TechnicienId { get; set; }
    public string TechnicienNom { get; set; } = string.Empty;
    public StatutIntervention Statut { get; set; }
    public CompteRenduDto? CompteRendu { get; set; }
    public InterventionDto() { }
    public InterventionDto(Intervention intervention)
    {
        Id = intervention.Id;
        DatePrevue = intervention.DatePrevue;
        DemandeId = intervention.DemandeId;
        TechnicienId = intervention.TechnicienId;
        TechnicienNom = intervention.Technicien?.Nom ?? string.Empty;
        Statut = intervention.Statut;

        CompteRendu = intervention.CompteRendu is null
            ? null
            : new CompteRenduDto(intervention.CompteRendu);
    }
}


public class UpdateInterventionDto
{
    public DateTime DatePrevue { get; set; }
}



public class TerminerInterventionDto
{
    public string OperationsEffectuees { get; set; } = string.Empty;

    public string Observations { get; set; } = string.Empty;

    public ResultatIntervention Resultat { get; set; }

    public string? Recommandations { get; set; }
}
