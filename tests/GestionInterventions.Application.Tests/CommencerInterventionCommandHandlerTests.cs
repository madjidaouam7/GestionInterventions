using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Interventions.Commands.CommencerIntervention;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class CommencerInterventionCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandInterventionExiste_EtEstPlanifiee_CommenceInterventionEtDemarreMaintenance()
    {
        // Arrange
        var equipement = new Equipement(
            "PC Dell",
            "SN123",
            "Ordinateur portable",
            "Bureau 1",
            1
        );

        // L'équipement doit être en panne avant de démarrer la maintenance
        equipement.SignalerPanne();

        var demande = new DemandeIntervention(
            "Ordinateur en panne",
            1,
            PrioriteDemande.Normale
        );

        demande.Accepter();

        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        var idProperty = typeof(Intervention)
        .GetProperty(nameof(Intervention.Id))!;
        idProperty.SetValue(intervention, 1);

        // Préparer les relations privées
        var equipementProperty = typeof(DemandeIntervention)
            .GetProperty(nameof(DemandeIntervention.Equipement))!;

        equipementProperty.SetValue(demande, equipement);

        var demandeProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Demande))!;

        demandeProperty.SetValue(intervention, demande);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(intervention);

        var handler = new CommencerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CommencerInterventionCommand(1);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(1, result);

        // L'intervention doit être EnCours
        Assert.Equal(
            StatutIntervention.EnCours,
            intervention.Statut
        );

        // L'équipement doit être en maintenance
        Assert.Equal(
            StatutEquipement.EnMaintenance,
            equipement.Statut
        );

        // L'intervention doit avoir été recherchée
        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        // Les modifications doivent être sauvegardées
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()
            ),
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
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Intervention?)null);

        var handler = new CommencerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CommencerInterventionCommand(1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(
                    command,
                    CancellationToken.None
                );
            }
        );

        Assert.Equal(
            "Aucune intervention trouvé avec l'id 1.",
            exception.Message
        );

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionEstDejaEnCours_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        // On prépare l'intervention dans l'état EnCours
        intervention.Commencer();

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(intervention);

        var handler = new CommencerInterventionCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CommencerInterventionCommand(1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(
                    command,
                    CancellationToken.None
                );
            }
        );

        Assert.Equal(
            "Une intervention ne peut pas commencer si elle n'est pas planifiée.",
            exception.Message
        );

        // Rien ne doit être sauvegardé
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }
}