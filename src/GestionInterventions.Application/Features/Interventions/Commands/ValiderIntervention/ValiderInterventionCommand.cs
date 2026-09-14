using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.ValiderIntervention;

public record ValiderInterventionCommand(int InterventionId) : IRequest<int>;
