using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Interventions.Commands.DemanderCorrectionIntervention;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class DemanderCorrectionInterventionCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandInterventionEstTermineeEtEquipementEstEnPanne_DemandeCorrectionEtDemarreMaintenance()
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
        intervention.Terminer(
            new CompteRendu(
                "Remplacement du composant défectueux",
                "La panne persiste.",
                ResultatIntervention.Echec,
                null
            )
        );

        var demandeProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Demande))!;
        demandeProperty.SetValue(intervention, demande);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new DemanderCorrectionInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new DemanderCorrectionInterventionCommand(1);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(intervention.Id, result);
        Assert.Equal(StatutIntervention.EnCours, intervention.Statut);
        Assert.Equal(StatutEquipement.EnMaintenance, equipement.Statut);

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionEstTermineeEtEquipementNestPasEnPanne_DemandeCorrectionEtSauvegarde()
    {
        // Arrange
        var equipement = new Equipement(
            "PC Dell",
            "SN123",
            "Ordinateur portable",
            "Bureau 1",
            1
        );

        var demande = new DemandeIntervention(
            "Contrôle du matériel",
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
        intervention.Terminer(
            new CompteRendu(
                "Vérification du matériel",
                "Le matériel est fonctionnel.",
                ResultatIntervention.Succes,
                null
            )
        );

        var demandeProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Demande))!;
        demandeProperty.SetValue(intervention, demande);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new DemanderCorrectionInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new DemanderCorrectionInterventionCommand(1);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(intervention.Id, result);
        Assert.Equal(StatutIntervention.EnCours, intervention.Statut);
        Assert.Equal(StatutEquipement.Fonctionnel, equipement.Statut);

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionNExistePas_LeverUneException()
    {
        // Arrange
        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Intervention?)null);

        var handler = new DemanderCorrectionInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new DemanderCorrectionInterventionCommand(1);

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Aucune intervention trouvé avec l'id 1.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionNestPasTerminee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new DemanderCorrectionInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new DemanderCorrectionInterventionCommand(1);

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Une correction ne peut être demandée que pour une intervention déjà terminée.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}