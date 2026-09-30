using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using MediatR;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Application.Features.Interventions.Commands.CreateIntervention;

public class CreateInterventionCommandHandler : IRequestHandler<CreateInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly ITechnicienRepository _technicienRepository;
    private readonly IDemandeInterventionRepository _demandeInterventionRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly GestionInterventions.Application.Common.Interfaces.INotificationPublisher _notificationPublisher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInterventionCommandHandler(IInterventionRepository interventionRepository, ITechnicienRepository technicienRepository, IDemandeInterventionRepository demandeInterventionRepository, INotificationRepository notificationRepository, IUnitOfWork unitOfWork, GestionInterventions.Application.Common.Interfaces.INotificationPublisher notificationPublisher)
    {
        _interventionRepository = interventionRepository;
        _demandeInterventionRepository = demandeInterventionRepository;
        _technicienRepository = technicienRepository;
        _notificationRepository = notificationRepository;
        _notificationPublisher = notificationPublisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateInterventionCommand request, CancellationToken cancellationToken)
    {
        var demandeIntervention = await _demandeInterventionRepository.GetByIdAsync(request.DemandeId, cancellationToken);
        if (demandeIntervention is null)
            throw new NotFoundException("Aucune demande trouvé avec cet id.");


        var technicien = await _technicienRepository.GetByIdAsync(request.TechnicienId, cancellationToken);
        if (technicien is null)
            throw new NotFoundException("Aucun technicien trouvé avec cet id.");


        if (demandeIntervention.Statut != StatutDemande.Acceptee)
            throw new DomainException("La demande doit être acceptée avant de planifier une intervention.");


        var intervention = new Intervention(request.DemandeId, request.TechnicienId, request.DatePrevue);

        await _interventionRepository.AddAsync(intervention, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var notification = new Notification(technicien.IdentityUserId, "NouvelleIntervention", "Une nouvelle intervention vous a été planifiée.", request.DemandeId, intervention.Id);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationPublisher.PublierNouvelleInterventionAsync(
            intervention.Id,
            "Une nouvelle intervention vous a été planifiée.",
            cancellationToken);

        return intervention.Id;
    }
}
