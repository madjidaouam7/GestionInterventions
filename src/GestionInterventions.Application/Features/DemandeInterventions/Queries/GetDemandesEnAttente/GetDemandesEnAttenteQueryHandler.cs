using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetDemandesEnAttente;

public class GetDemandesEnAttenteQueryHandler : IRequestHandler<GetDemandesEnAttenteQuery, List<DemandeInterventionDto>>
{
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;

    public GetDemandesEnAttenteQueryHandler(IDemandeInterventionRepository demandeInterventionRepository)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
    }

    public async Task<List<DemandeInterventionDto>> Handle(GetDemandesEnAttenteQuery request, CancellationToken cancellationToken)
    {

        var demandeInterventions = await _demandeInterventionRepository.GetEnAttenteAsync(cancellationToken);

        return demandeInterventions.Select(demandeIntervention => new DemandeInterventionDto(demandeIntervention))
                                   .ToList();

    }
}
