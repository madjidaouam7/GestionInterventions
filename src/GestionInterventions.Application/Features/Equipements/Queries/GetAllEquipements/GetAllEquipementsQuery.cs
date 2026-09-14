using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetAllEquipements;

public record GetAllEquipementsQuery() : IRequest<List<EquipementDto>>;
