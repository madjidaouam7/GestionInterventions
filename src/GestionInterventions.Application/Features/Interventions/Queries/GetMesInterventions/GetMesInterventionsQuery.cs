using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetMesInterventions;

public record GetMesInterventionsQuery(int TechnicienId) : IRequest<List<InterventionDto>>;
