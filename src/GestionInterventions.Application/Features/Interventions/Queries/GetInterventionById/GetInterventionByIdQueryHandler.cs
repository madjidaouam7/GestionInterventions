using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Queries.GetInterventionById;

public class GetInterventionByIdQueryHandler : IRequestHandler<GetInterventionByIdQuery, InterventionDto?>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInterventionByIdQueryHandler(IInterventionRepository interventionRepository, ICurrentUserService currentUserService)
    {
        _interventionRepository = interventionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<InterventionDto?> Handle(GetInterventionByIdQuery request, CancellationToken cancellationToken)
    {

        var intervention = await _interventionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (intervention is null)
            throw new NotFoundException($"Aucune intervention trouvée avec l'id {request.Id}.");


        var estResponsable = _currentUserService.Role == "Responsable";
        var estClientProprietaire = _currentUserService.Role == "Client"
            && _currentUserService.ClientId == intervention.Demande.Equipement.ClientId;
        var estTechnicienAssigne = _currentUserService.Role == "Technicien"
            && _currentUserService.TechnicienId == intervention.TechnicienId;

        if (!estResponsable && !estClientProprietaire && !estTechnicienAssigne)
            throw new ForbiddenAccessException("Vous n'êtes pas autorisé à consulter cette intervention.");


        return new InterventionDto(intervention);
    }
}
