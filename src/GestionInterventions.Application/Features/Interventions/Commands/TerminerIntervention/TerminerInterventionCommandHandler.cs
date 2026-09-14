using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.Features.Interventions.Commands.TerminerIntervention;

public class TerminerInterventionCommandHandler : IRequestHandler<TerminerInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TerminerInterventionCommandHandler(IInterventionRepository interventionRepository, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(TerminerInterventionCommand request, CancellationToken cancellationToken)
    {
        var intervention = await _interventionRepository.GetByIdAsync(request.InterventionId, cancellationToken);

        if (intervention is null)
            throw new NotFoundException("Aucune intervention trouvé avec cet id.");

        var compteRendu = new CompteRendu(
            request.OperationsEffectuees,
            request.Observations,
            request.Resultat,
            request.Recommandations);

        intervention.Terminer(compteRendu);

        if (request.Resultat == ResultatIntervention.Echec)
        {
            intervention.Demande.Equipement.SignalerEchecMaintenance();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return intervention.Id;
    }
}
