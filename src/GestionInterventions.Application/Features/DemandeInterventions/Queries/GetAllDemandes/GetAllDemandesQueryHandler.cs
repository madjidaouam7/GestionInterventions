using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Models;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetAllDemandes;

public class GetAllDemandesQueryHandler : IRequestHandler<GetAllDemandesQuery, PagedResult<DemandeInterventionDto>>
{
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly IInterventionRepository _interventionRepository;

    public GetAllDemandesQueryHandler(IDemandeInterventionRepository demandeInterventionRepository, IInterventionRepository interventionRepository)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
        _interventionRepository = interventionRepository;
    }

    public async Task<PagedResult<DemandeInterventionDto>> Handle(GetAllDemandesQuery request, CancellationToken cancellationToken)
    {
        var (demandes, totalCount) = await _demandeInterventionRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

        var result = new List<DemandeInterventionDto>();

        foreach (var demande in demandes)
        {
            var dto = new DemandeInterventionDto(demande);

            var intervention = await _interventionRepository.GetByDemandeIdAsync(demande.Id, cancellationToken);

            dto.InterventionId = intervention?.Id;
            dto.DatePrevue = intervention?.DatePrevue;
            dto.InterventionStatut = intervention?.Statut.ToString();

            result.Add(dto);
        }

        return new PagedResult<DemandeInterventionDto>
        {
            Items = result,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
