using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.CreateDemandeIntervention;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class CreateDemandeInterventionCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandEquipementExiste_CreeDemandeEtSignalePanneEtSauvegarde()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var notificationPublisherMock = new Mock<INotificationPublisher>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        adminConfigurationMock.SetupGet(x => x.Email).Returns("admin@example.com");
        identityServiceMock
            .Setup(x => x.GetUserIdByEmailAsync("admin@example.com"))
            .ReturnsAsync("responsable-id");

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipement);

        var handler = new CreateDemandeInterventionCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            notificationPublisherMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateDemandeInterventionCommand(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(StatutEquipement.EnPanne, equipement.Statut);

        equipementRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        demandeRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<DemandeIntervention>(d =>
                    d.Description == "Ordinateur chauffe" &&
                    d.EquipementId == 1 &&
                    d.Priorite == PrioriteDemande.Normale
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );

        notificationPublisherMock.Verify(
            x => x.PublierNouvelleDemandeAsync(
                result,
                "Une nouvelle demande a été déclarée.",
                It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandEquipementNExistePas_LeverUneException()
    {
        // Arrange
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var notificationPublisherMock = new Mock<INotificationPublisher>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipement?)null);

        var handler = new CreateDemandeInterventionCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            notificationPublisherMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateDemandeInterventionCommand("Ordinateur chauffe", 1, PrioriteDemande.Normale);

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        demandeRepositoryMock.Verify(
            x => x.AddAsync(
            It.IsAny<DemandeIntervention>(),
            It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );

        Assert.Equal("Aucun équipement trouvé avec l'id 1.", exception.Message);

    }


    [Fact]
    public async Task Handle_QuandEquipementEstDejaEnPanne_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // On prépare volontairement l'équipement dans l'état EnPanne
        equipement.SignalerPanne();

        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var notificationRepositoryMock = new Mock<INotificationRepository>();
        var identityServiceMock = new Mock<IIdentityService>();
        var adminConfigurationMock = new Mock<IAdminConfiguration>();
        var notificationPublisherMock = new Mock<INotificationPublisher>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipement);

        var handler = new CreateDemandeInterventionCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            notificationRepositoryMock.Object,
            identityServiceMock.Object,
            adminConfigurationMock.Object,
            notificationPublisherMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateDemandeInterventionCommand(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // Act et Assert
        await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        // La demande ne doit pas être créée
        demandeRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<DemandeIntervention>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        // Rien ne doit être sauvegardé
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}