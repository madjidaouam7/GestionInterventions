using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetAllDemandes;

public record GetAllDemandesQuery() : IRequest<List<DemandeInterventionDto>>;