using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.CreateDemandeIntervention;

public class CreateDemandeInterventionCommandHandler : IRequestHandler<CreateDemandeInterventionCommand, int>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDemandeInterventionCommandHandler(IDemandeInterventionRepository demandeInterventionRepository,IEquipementRepository equipementRepository, IUnitOfWork unitOfWork)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
        _equipementRepository = equipementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateDemandeInterventionCommand request, CancellationToken cancellationToken)
    {
        var equipement = await _equipementRepository.GetByIdAsync(request.EquipementId, cancellationToken);

        if (equipement is null)
            throw new NotFoundException($"Aucun équipement trouvé avec l'id {request.EquipementId}.");

        equipement.SignalerPanne();

        var demandeIntervention = new DemandeIntervention(request.Description, request.EquipementId, request.Priorite);

        await _demandeInterventionRepository.AddAsync(demandeIntervention, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return demandeIntervention.Id;
    }
}
