using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetHistoriqueEquipement;

public class GetHistoriqueEquipementQueryHandler : IRequestHandler<GetHistoriqueEquipementQuery, HistoriqueEquipementDto>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetHistoriqueEquipementQueryHandler(IEquipementRepository equipementRepository, ICurrentUserService currentUserService)
    {
        _equipementRepository = equipementRepository;
        _currentUserService = currentUserService;
    }

    public async Task<HistoriqueEquipementDto> Handle(GetHistoriqueEquipementQuery request, CancellationToken cancellationToken)
    {

        var equipement = await _equipementRepository.GetHistoriqueAsync(request.EquipementId, cancellationToken);

        if (equipement is null)
            throw new NotFoundException($"Aucun équipement trouvé avec l'id {request.EquipementId}.");

        // Un Client ne peut consulter que l'historique son propre historique d'equipement
        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != equipement.ClientId)
        {
            throw new ForbiddenAccessException("Vous ne pouvez consulter que l'historique de votre propre equipement.");
        }

        return new HistoriqueEquipementDto(equipement);
    }
}
