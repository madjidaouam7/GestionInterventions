using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetAllInterventions;

public record GetAllInterventionsQuery() : IRequest<List<InterventionDto>>;
