using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Interventions.Commands.ModifierPlanification;

public class ModifierPlanificationCommandHandler : IRequestHandler<ModifierPlanificationCommand, InterventionDto>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModifierPlanificationCommandHandler(IInterventionRepository interventionRepository, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InterventionDto> Handle(ModifierPlanificationCommand request, CancellationToken cancellationToken)
    {

        var intervention = await _interventionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (intervention is null)
             throw new NotFoundException("Aucune intervention trouvée avec cet id.");

        intervention.ModifierDatePrevue(request.NouvelleDatePrevue);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new InterventionDto
        {
            Id = intervention.Id,
            DatePrevue = intervention.DatePrevue,
            DemandeId = intervention.DemandeId,
            TechnicienId = intervention.TechnicienId,
            Statut = intervention.Statut
        };
    }
}
