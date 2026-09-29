using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Models;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Queries.GetEquipementsByClient;

public class GetEquipementsByClientQueryHandler : IRequestHandler<GetEquipementsByClientQuery, PagedResult<EquipementDto>>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetEquipementsByClientQueryHandler(IEquipementRepository equipementRepository, IClientRepository clientRepository, ICurrentUserService currentUserService)
    {
        _equipementRepository = equipementRepository;
        _clientRepository = clientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<EquipementDto>> Handle(GetEquipementsByClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);

        if (client is null)
            throw new NotFoundException("Le client spécifié n'existe pas.");


        var (equipements, totalCount) = await _equipementRepository.GetByClientIdAsync(request.ClientId, request.Page, request.PageSize, cancellationToken);

        // Un Client ne peut consulter que ses propre equipements
        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != client.Id)
        {
            throw new ForbiddenAccessException("Vous ne pouvez consulter que vos propre equipements.");
        }

        return new PagedResult<EquipementDto>
        {
            Items = equipements.Select(equipement => new EquipementDto(equipement)).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

    }
}
