using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Interventions.Commands.TerminerIntervention;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class TerminerInterventionCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandInterventionEstEnCoursEtResultatSucces_TermineInterventionEtSauvegarde()
    {
        // Arrange
        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        intervention.Commencer();

        var idProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Id))!;
        idProperty.SetValue(intervention, 1);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var notificationPublisherMock = new Mock<INotificationPublisher>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        identityServiceMock
            .Setup(x => x.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync("responsable-user-id");
        adminConfigurationMock
            .SetupGet(x => x.Email)
            .Returns("responsable@example.com");

        var handler = new TerminerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            unitOfWorkMock.Object,
            notificationPublisherMock.Object
        );

        var command = new TerminerInterventionCommand(
            1,
            "Remplacement du composant défectueux",
            "Le matériel fonctionne correctement.",
            ResultatIntervention.Succes,
            "Prévoir un contrôle dans six mois."
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(1, result);
        Assert.Equal(StatutIntervention.Terminee, intervention.Statut);
        Assert.NotNull(intervention.CompteRendu);
        Assert.Equal(
            "Remplacement du composant défectueux",
            intervention.CompteRendu.OperationsEffectuees
        );
        Assert.Equal(
            "Le matériel fonctionne correctement.",
            intervention.CompteRendu.Observations
        );
        Assert.Equal(
            ResultatIntervention.Succes,
            intervention.CompteRendu.Resultat
        );
        Assert.Equal(
            "Prévoir un contrôle dans six mois.",
            intervention.CompteRendu.Recommandations
        );

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );

        notificationPublisherMock.Verify(
            x => x.PublierInterventionTermineeAsync(
                result,
                "Une intervention est terminée et attend votre validation.",
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionEstEnCoursEtResultatEchec_SignaleEchecMaintenanceEtSauvegarde()
    {
        // Arrange
        var equipement = new Equipement(
            "PC Dell",
            "SN123",
            "Ordinateur portable",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();
        equipement.DemarrerMaintenance();

        var demande = new DemandeIntervention(
            "Ordinateur en panne",
            1,
            PrioriteDemande.Normale
        );

        var equipementProperty = typeof(DemandeIntervention)
            .GetProperty(nameof(DemandeIntervention.Equipement))!;
        equipementProperty.SetValue(demande, equipement);

        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        intervention.Commencer();

        var demandeProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Demande))!;
        demandeProperty.SetValue(intervention, demande);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        identityServiceMock
            .Setup(x => x.GetUserIdByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync("responsable-user-id");
        adminConfigurationMock
            .SetupGet(x => x.Email)
            .Returns("responsable@example.com");

        var handler = new TerminerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<INotificationPublisher>()
        );

        var command = new TerminerInterventionCommand(
            1,
            "Remplacement du composant défectueux",
            "La panne persiste après intervention.",
            ResultatIntervention.Echec,
            null
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(0, result);
        Assert.Equal(StatutIntervention.Terminee, intervention.Statut);
        Assert.Equal(StatutEquipement.EnPanne, equipement.Statut);
        Assert.NotNull(intervention.CompteRendu);
        Assert.Equal(ResultatIntervention.Echec, intervention.CompteRendu.Resultat);

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionNExistePas_LeverUneException()
    {
        // Arrange
        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Intervention?)null);

        var handler = new TerminerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<INotificationPublisher>()
        );

        var command = new TerminerInterventionCommand(
            1,
            "Remplacement du composant défectueux",
            "La panne persiste.",
            ResultatIntervention.Echec,
            null
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Aucune intervention trouvé avec cet id.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionNestPasEnCours_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new TerminerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            unitOfWorkMock.Object,
            Mock.Of<INotificationPublisher>()
        );

        var command = new TerminerInterventionCommand(
            1,
            "Remplacement du composant défectueux",
            "La panne persiste.",
            ResultatIntervention.Succes,
            null
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Une intervention ne peut se terminer que si elle est en cours.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}