using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Models;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetMesDemandes;

public class GetMesDemandesQueryHandler : IRequestHandler<GetMesDemandesQuery, PagedResult<DemandeInterventionDto>>
{
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetMesDemandesQueryHandler(IDemandeInterventionRepository demandeInterventionRepository, IClientRepository clientRepository, ICurrentUserService currentUserService)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
        _clientRepository = clientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<DemandeInterventionDto>> Handle(GetMesDemandesQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);

        if (client is null)
            throw new NotFoundException("Le client spécifié n'existe pas.");

        var (demandeInterventions, totalCount) = await _demandeInterventionRepository.GetByClientIdAsync(request.ClientId, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<DemandeInterventionDto>
        {
            Items = demandeInterventions.Select(demandeIntervention => new DemandeInterventionDto(demandeIntervention)).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

    }
}
