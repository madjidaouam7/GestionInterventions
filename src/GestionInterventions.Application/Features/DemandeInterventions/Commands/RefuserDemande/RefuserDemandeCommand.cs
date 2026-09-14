using GestionInterventions.Domain.Enums;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.RefuserDemande;

public record RefuserDemandeCommand(int DemandeInterventionId) : IRequest<int>;