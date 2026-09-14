using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Entities;

public class CompteRendu
{
    public string OperationsEffectuees { get; private set; } = string.Empty;
    public string Observations { get; private set; } = string.Empty;
    public ResultatIntervention Resultat { get; private set; }
    public string? Recommandations { get; private set; }

    private CompteRendu() { } // Pour EF Core

    public CompteRendu(string operationsEffectuees, string observations, ResultatIntervention resultat, string? recommandations)
    {
        if (string.IsNullOrWhiteSpace(operationsEffectuees))
            throw new DomainException("Les opérations effectuées doivent être renseignées.");

        if (!Enum.IsDefined(resultat))
            throw new DomainException("Le résultat de l'intervention est invalide.");

        OperationsEffectuees = operationsEffectuees;
        Observations = observations;
        Resultat = resultat;
        Recommandations = recommandations;
    }
}