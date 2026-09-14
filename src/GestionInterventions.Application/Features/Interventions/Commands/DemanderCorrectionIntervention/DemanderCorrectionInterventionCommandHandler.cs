using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.Features.Interventions.Commands.DemanderCorrectionIntervention;

public class DemanderCorrectionInterventionCommandHandler : IRequestHandler<DemanderCorrectionInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DemanderCorrectionInterventionCommandHandler(IInterventionRepository interventionRepository, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(DemanderCorrectionInterventionCommand request, CancellationToken cancellationToken)
    {
        var intervention = await _interventionRepository.GetByIdAsync(request.InterventionId, cancellationToken);

        if (intervention is null)
            throw new NotFoundException($"Aucune intervention trouvé avec l'id {request.InterventionId}.");

        intervention.DemanderCorrection();

        if (intervention.Demande.Equipement.Statut == StatutEquipement.EnPanne)
        {
            intervention.Demande.Equipement.DemarrerMaintenance();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return intervention.Id;
    }
}