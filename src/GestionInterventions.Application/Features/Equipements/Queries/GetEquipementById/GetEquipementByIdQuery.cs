using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetEquipementById;

public record GetEquipementByIdQuery(int Id) : IRequest<EquipementDto?>;