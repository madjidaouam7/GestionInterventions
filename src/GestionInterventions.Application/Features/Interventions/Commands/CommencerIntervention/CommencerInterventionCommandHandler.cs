using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.CommencerIntervention;

public class CommencerInterventionCommandHandler : IRequestHandler<CommencerInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CommencerInterventionCommandHandler(IInterventionRepository interventionRepository, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CommencerInterventionCommand request, CancellationToken cancellationToken)
    {
        var intervention = await _interventionRepository.GetByIdAsync(request.InterventionId, cancellationToken);

        if (intervention is null)
            throw new NotFoundException($"Aucune intervention trouvé avec l'id {request.InterventionId}.");

        intervention.Commencer();
        intervention.Demande.Equipement.DemarrerMaintenance();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return intervention.Id;
    }
}
