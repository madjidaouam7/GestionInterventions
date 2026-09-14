using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetMesDemandes;

public record GetMesDemandesQuery(int ClientId) : IRequest<List<DemandeInterventionDto>>;