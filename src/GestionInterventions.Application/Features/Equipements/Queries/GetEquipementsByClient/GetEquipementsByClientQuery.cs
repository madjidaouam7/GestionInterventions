using GestionInterventions.Application.Common.Models;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetEquipementsByClient;

public record GetEquipementsByClientQuery(int ClientId, int Page = 1, int PageSize = 10) : IRequest<PagedResult<EquipementDto>>;
