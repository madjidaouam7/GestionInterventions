using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using MediatR;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.Features.Interventions.Commands.TerminerIntervention;

public class TerminerInterventionCommandHandler : IRequestHandler<TerminerInterventionCommand, int>
{
    private readonly IInterventionRepository _interventionRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IIdentityService _identityService;
    private readonly IAdminConfiguration _adminConfiguration;
    private readonly IUnitOfWork _unitOfWork;

    public TerminerInterventionCommandHandler(IInterventionRepository interventionRepository, INotificationRepository notificationRepository, IIdentityService identityService, IAdminConfiguration adminConfiguration, IUnitOfWork unitOfWork)
    {
        _interventionRepository = interventionRepository;
        _notificationRepository = notificationRepository;
        _identityService = identityService;
        _adminConfiguration = adminConfiguration;
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

        var responsableUserId = await _identityService.GetUserIdByEmailAsync(_adminConfiguration.Email);
        if (string.IsNullOrWhiteSpace(responsableUserId))
            throw new NotFoundException("Le compte Responsable n'a pas été trouvé.");

        var notification = new Notification(responsableUserId, "InterventionTerminee", "Une intervention est terminée et attend votre validation.", null, intervention.Id);
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return intervention.Id;
    }
}
