using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Commands.AccepterDemande;

public class AccepterDemandeCommandHandler : IRequestHandler<AccepterDemandeCommand, int>
{
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccepterDemandeCommandHandler(IDemandeInterventionRepository demandeInterventionRepository, IUnitOfWork unitOfWork)
    {
        _demandeInterventionRepository = demandeInterventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(AccepterDemandeCommand request, CancellationToken cancellationToken)
    {
        var demande = await _demandeInterventionRepository.GetByIdAsync(request.DemandeInterventionId, cancellationToken);

        if (demande is null)
            throw new NotFoundException("Aucune demande existe avec cet id.");

        demande.Accepter();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return demande.Id;
    }
}