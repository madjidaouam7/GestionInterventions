using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.AccepterDemande;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class AccepterDemandeCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandDemandeExiste_AccepterDemandeEtSauvegardeEtAppelleLesBonnesMethodes()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        var handler = new AccepterDemandeCommandHandler(
            demandeRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var command = new AccepterDemandeCommand(1);
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(demande.Id, result);
        Assert.Equal(StatutDemande.Acceptee, demande.Statut);

        demandeRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task handle_QuandDemandeNExistePas_LeverUneException()
    {
        // Arrange
        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DemandeIntervention?)null);

        var handler = new AccepterDemandeCommandHandler(
            demandeRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new AccepterDemandeCommand(1);

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("Aucune demande existe avec cet id.", exception.Message);
    }


    [Fact]
    public async Task Handle_QuandDemandeEstDejaAcceptee_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // On met volontairement la demande dans l'état Acceptee 
        demande.Accepter();

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        var handler = new AccepterDemandeCommandHandler(
            demandeRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new AccepterDemandeCommand(1);

        // Act et Assert
        await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}