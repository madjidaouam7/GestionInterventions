using GestionInterventions.Application.Common.Models;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetMesDemandes;

public record GetMesDemandesQuery(int ClientId, int Page = 1, int PageSize = 10) : IRequest<PagedResult<DemandeInterventionDto>>;
