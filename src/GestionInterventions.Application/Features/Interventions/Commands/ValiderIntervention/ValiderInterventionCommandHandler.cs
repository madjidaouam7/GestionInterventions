using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Application.Features.Interventions.Commands.ValiderIntervention;

public class ValiderInterventionCommandHandler : IRequestHandler<ValiderInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ValiderInterventionCommandHandler(IInterventionRepository interventionRepository, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ValiderInterventionCommand request, CancellationToken cancellationToken)
    {
        var intervention = await _interventionRepository.GetByIdAsync(request.InterventionId, cancellationToken);

        if (intervention is null)
            throw new NotFoundException($"Aucune intervention trouvé avec l'id {request.InterventionId}.");

        if (intervention.CompteRendu?.Resultat != ResultatIntervention.Succes)
        {
            throw new DomainException("Seule une intervention réussie peut être validée.");
        }
        
        intervention.Valider();
        intervention.Demande.Equipement.RemettreEnService();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return intervention.Id;
    }
}