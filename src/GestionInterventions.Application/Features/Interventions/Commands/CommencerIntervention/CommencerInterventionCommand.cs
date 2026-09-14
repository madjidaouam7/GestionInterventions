using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.CommencerIntervention;

public record CommencerInterventionCommand(int InterventionId) : IRequest<int>;
