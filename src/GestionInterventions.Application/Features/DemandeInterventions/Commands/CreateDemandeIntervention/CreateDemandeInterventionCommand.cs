using GestionInterventions.Domain.Enums;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.CreateDemandeIntervention;

public record CreateDemandeInterventionCommand(
    string Description,
    int EquipementId,
    PrioriteDemande Priorite
) : IRequest<int>;
