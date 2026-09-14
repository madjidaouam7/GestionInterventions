using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.ModifierPlanification;

public record ModifierPlanificationCommand(
    int Id,
    DateTime NouvelleDatePrevue
) : IRequest<InterventionDto>;
