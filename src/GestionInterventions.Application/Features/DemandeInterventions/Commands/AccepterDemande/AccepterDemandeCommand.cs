using GestionInterventions.Domain.Enums;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.AccepterDemande;

public record AccepterDemandeCommand(int DemandeInterventionId) : IRequest<int>;
