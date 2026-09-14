using GestionInterventions.Domain.Enums;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.TerminerIntervention;

public record TerminerInterventionCommand(int InterventionId, string OperationsEffectuees, string Observations, ResultatIntervention Resultat, string? Recommandations) : IRequest<int>;
