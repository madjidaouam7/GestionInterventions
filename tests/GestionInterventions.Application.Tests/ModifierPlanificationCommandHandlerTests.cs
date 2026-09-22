using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Interventions.Commands.ModifierPlanification;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class ModifierPlanificationCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandInterventionExisteEtEstPlanifiee_ModifieDateEtSauvegarde()
    {
        // Arrange
        var dateInitiale = DateTime.UtcNow.AddDays(1);
        var nouvelleDate = DateTime.UtcNow.AddDays(2);

        var intervention = new Intervention(
            1,
            1,
            dateInitiale
        );

        var idProperty = typeof(Intervention)
            .GetProperty(nameof(Intervention.Id))!;
        idProperty.SetValue(intervention, 1);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new ModifierPlanificationCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new ModifierPlanificationCommand(
            1,
            nouvelleDate
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(nouvelleDate, result.DatePrevue);
        Assert.Equal(1, result.DemandeId);
        Assert.Equal(1, result.TechnicienId);
        Assert.Equal(StatutIntervention.Planifiee, result.Statut);
        Assert.Equal(nouvelleDate, intervention.DatePrevue);

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

        var handler = new ModifierPlanificationCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new ModifierPlanificationCommand(
            1,
            DateTime.UtcNow.AddDays(1)
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Aucune intervention trouvée avec cet id.",
            exception.Message
        );

        interventionRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandInterventionEstDejaCommencee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            1,
            1,
            DateTime.UtcNow.AddDays(1)
        );

        intervention.Commencer();

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        interventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intervention);

        var handler = new ModifierPlanificationCommandHandler(
            interventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new ModifierPlanificationCommand(
            1,
            DateTime.UtcNow.AddDays(2)
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Impossible de modifier la planification d'une intervention qui a déjà commencé ou terminé.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}