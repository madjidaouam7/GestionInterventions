using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetEquipementsByClient;

public record GetEquipementsByClientQuery(int ClientId) : IRequest<List<EquipementDto>>;
