using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetInterventionById;

public record GetInterventionByIdQuery(int Id) : IRequest<InterventionDto?>;