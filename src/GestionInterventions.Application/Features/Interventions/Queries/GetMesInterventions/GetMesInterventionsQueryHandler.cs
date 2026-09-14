using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetMesInterventions;

public class GetMesInterventionsQueryHandler : IRequestHandler<GetMesInterventionsQuery, List<InterventionDto>>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly ITechnicienRepository _technicienRepository;
    private readonly ICurrentUserService _currentUserService;


    public GetMesInterventionsQueryHandler(IInterventionRepository interventionRepository, ITechnicienRepository technicienRepository, ICurrentUserService currentUserService)
    {
        _interventionRepository = interventionRepository;
        _technicienRepository = technicienRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<InterventionDto>> Handle(GetMesInterventionsQuery request, CancellationToken cancellationToken)
    {
        var technicien = await _technicienRepository.GetByIdAsync(request.TechnicienId, cancellationToken);

        if (technicien is null)
            throw new NotFoundException("Le technicien spécifié n'existe pas.");

        var interventions = await _interventionRepository.GetByTechnicienIdAsync(request.TechnicienId, cancellationToken);

        return interventions.Select(intervention => new InterventionDto(intervention))
                            .ToList();

    }
}
