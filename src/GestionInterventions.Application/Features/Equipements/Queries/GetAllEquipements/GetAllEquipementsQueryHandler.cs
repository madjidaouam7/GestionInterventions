using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetAllEquipements;

public class GetAllEquipementsQueryHandler : IRequestHandler<GetAllEquipementsQuery, List<EquipementDto>>
{
    private readonly IEquipementRepository _equipementRepository;

    public GetAllEquipementsQueryHandler(IEquipementRepository equipementRepository)
    {
        _equipementRepository = equipementRepository;
    }

    public async Task<List<EquipementDto>> Handle(GetAllEquipementsQuery request, CancellationToken cancellationToken)
    {
        var equipements = await _equipementRepository.GetAllAsync(cancellationToken);
        return equipements.Select(e => new EquipementDto(e)).ToList();
    }
}