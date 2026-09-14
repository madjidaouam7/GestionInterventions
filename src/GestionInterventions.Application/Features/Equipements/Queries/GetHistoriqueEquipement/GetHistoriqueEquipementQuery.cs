using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetHistoriqueEquipement;

public record GetHistoriqueEquipementQuery(int EquipementId) : IRequest<HistoriqueEquipementDto>;