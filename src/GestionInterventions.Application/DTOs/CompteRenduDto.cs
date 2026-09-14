using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.DTOs;

public class CompteRenduDto
{
    public string OperationsEffectuees { get; set; } = string.Empty;

    public string Observations { get; set; } = string.Empty;

    public ResultatIntervention Resultat { get; set; }

    public string? Recommandations { get; set; }
    public CompteRenduDto() { }

    public CompteRenduDto(CompteRendu compteRendu)
    {
        OperationsEffectuees = compteRendu.OperationsEffectuees;
        Observations = compteRendu.Observations;
        Resultat = compteRendu.Resultat;
        Recommandations = compteRendu.Recommandations;
    }
}
