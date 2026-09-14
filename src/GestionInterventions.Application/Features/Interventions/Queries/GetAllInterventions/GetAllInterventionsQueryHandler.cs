using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetAllInterventions;

public class GetAllInterventionsQueryHandler : IRequestHandler<GetAllInterventionsQuery, List<InterventionDto>>
{
    private readonly IInterventionRepository _interventionRepository;

    public GetAllInterventionsQueryHandler(IInterventionRepository interventionRepository)
    {
        _interventionRepository = interventionRepository;
    }

    public async Task<List<InterventionDto>> Handle(GetAllInterventionsQuery request, CancellationToken cancellationToken)
    {
        var interventions = await _interventionRepository.GetAllAsync(cancellationToken);
        return interventions.Select(i => new InterventionDto(i)).ToList();
    }
}