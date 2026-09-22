using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.RefuserDemande;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class RefuserDemandeCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandDemandeEtEquipementExiste_RefuseDemandeEtSauvegarde()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipement);

        var handler = new RefuserDemandeCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        // Act
        var command = new RefuserDemandeCommand(1);

        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(demande.Id, result);
        Assert.Equal(StatutDemande.Refusee, demande.Statut);
        Assert.Equal(StatutEquipement.Fonctionnel, equipement.Statut);

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }



    [Fact]
    public async Task Handle_QuandDemandeNExistePas_LeverUneException()
    {
        // Arrange
        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DemandeIntervention?)null);

        var handler = new RefuserDemandeCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RefuserDemandeCommand(1);

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
    public async Task Handle_QuandEquipementNExistePas_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
                    "Ordinateur chauffe",
                    1,
                    PrioriteDemande.Normale
                );

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipement?)null);

        var handler = new RefuserDemandeCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RefuserDemandeCommand(1);

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("Aucun equipement existe avec cet id.", exception.Message);
    }


    [Fact]
    public async Task Handle_QuandDemandeEstDejaRefusee_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // On met volontairement la demande dans l'état Refusee 
        demande.Refuser();

        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipement);

        var handler = new RefuserDemandeCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RefuserDemandeCommand(1);

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


    [Fact]
    public async Task Handle_QuandDemandeEtEquipementExiste_AppelleLesBonnesMethodes()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();

        var demandeRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        equipementRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipement);

        var handler = new RefuserDemandeCommandHandler(
            demandeRepositoryMock.Object,
            equipementRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RefuserDemandeCommand(1);
        
        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        demandeRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );

        equipementRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
            Times.Once
        );
        
        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}