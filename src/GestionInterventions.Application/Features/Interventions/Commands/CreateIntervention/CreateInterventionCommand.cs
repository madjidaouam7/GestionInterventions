using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.CreateIntervention;

public record CreateInterventionCommand(
    int DemandeId,
    int TechnicienId,
    DateTime DatePrevue
) : IRequest<int>;
