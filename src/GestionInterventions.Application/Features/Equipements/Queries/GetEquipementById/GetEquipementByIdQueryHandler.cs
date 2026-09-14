using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetEquipementById;

public class GetEquipementByIdQueryHandler : IRequestHandler<GetEquipementByIdQuery, EquipementDto?>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetEquipementByIdQueryHandler(IEquipementRepository equipementRepository, ICurrentUserService currentUserService)
    {
        _equipementRepository = equipementRepository;
        _currentUserService = currentUserService;
    }

    public async Task<EquipementDto?> Handle(GetEquipementByIdQuery request, CancellationToken cancellationToken)
    {

        var equipement = await _equipementRepository.GetByIdAsync(request.Id, cancellationToken);

        if (equipement == null)
        {
            throw new NotFoundException($"Aucun equipement trouvé avec l'id {request.Id}.");
        }

        // Un Client ne peut consulter que son equipement
        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != equipement.ClientId)
        {
            throw new ForbiddenAccessException("Vous ne pouvez consulter que votre propre equipement.");
        }

        return new EquipementDto(equipement);

    }
}