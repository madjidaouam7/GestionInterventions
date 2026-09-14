using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.DemanderCorrectionIntervention;

public record DemanderCorrectionInterventionCommand(int InterventionId) : IRequest<int>;
