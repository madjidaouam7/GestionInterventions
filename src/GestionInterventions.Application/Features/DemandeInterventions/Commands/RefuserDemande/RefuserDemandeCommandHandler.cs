using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.RefuserDemande;

public class RefuserDemandeCommandHandler : IRequestHandler<RefuserDemandeCommand, int>
{
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly IEquipementRepository _equipementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefuserDemandeCommandHandler(IDemandeInterventionRepository demandeInterventionRepository, IEquipementRepository equipementRepository, IUnitOfWork unitOfWork)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
        _equipementRepository = equipementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RefuserDemandeCommand request, CancellationToken cancellationToken)
    {
        var demande = await _demandeInterventionRepository.GetByIdAsync(request.DemandeInterventionId, cancellationToken);

        if (demande is null)
            throw new NotFoundException("Aucune demande existe avec cet id.");

        var equipement = await _equipementRepository.GetByIdAsync(demande.EquipementId, cancellationToken);

        if (equipement is null)
            throw new NotFoundException("Aucun equipement existe avec cet id.");

        demande.Refuser();
        equipement.AnnulerSignalementPanne();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return demande.Id;
    }
}