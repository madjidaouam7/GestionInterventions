using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Interventions.Commands.CreateIntervention;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class CreateInterventionCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandDemandeExisteEtTechExisteEtDemandeAcceptee_CreeInterventionEtSauvegarde()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        demande.Accepter();

        var technicien = new Technicien(
            "tech1",
            "tech1@gmail.com",
            "0123456789",
            "Alger",
            "identity-user-123"
        );

        var datePrevue = DateTime.UtcNow.AddDays(1);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var demandeInterventionRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeInterventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        technicienRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(technicien);

        var handler = new CreateInterventionCommandHandler(
            interventionRepositoryMock.Object,
            technicienRepositoryMock.Object,
            demandeInterventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateInterventionCommand(
            1,
            1,
            datePrevue
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert 
        interventionRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Intervention>(i =>
                    i.DemandeId == 1 &&
                    i.TechnicienId == 1 &&
                    i.DatePrevue == datePrevue
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandDemandeNExistePas_LeverUneException()
    {
        // Arrange
        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var demandeInterventionRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeInterventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DemandeIntervention?)null);

        var handler = new CreateInterventionCommandHandler(
            interventionRepositoryMock.Object,
            technicienRepositoryMock.Object,
            demandeInterventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateInterventionCommand(
            1,
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

        Assert.Equal("Aucune demande trouvé avec cet id.", exception.Message);

        // Le technicien ne doit pas être recherché
        technicienRepositoryMock.Verify(
            x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        // L'intervention ne doit pas être créé
        interventionRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Intervention>(),
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


    [Fact]
    public async Task Handle_QuandTechnicienNExistePas_LeverUneException()
    {
        // Arrange

        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var demandeInterventionRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeInterventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        technicienRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Technicien?)null);

        var handler = new CreateInterventionCommandHandler(
            interventionRepositoryMock.Object,
            technicienRepositoryMock.Object,
            demandeInterventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateInterventionCommand(
            1,
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

        Assert.Equal("Aucun technicien trouvé avec cet id.", exception.Message);

        // L'équipement ne doit pas être créé
        interventionRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Intervention>(),
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

    [Fact]
    public async Task Handle_QuandDemandeNEstPasAcceptee_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        var technicien = new Technicien(
            "tech1",
            "tech1@gmail.com",
            "0123456789",
            "Alger",
            "identity-user-123"
        );

        var datePrevue = DateTime.UtcNow.AddDays(1);

        var interventionRepositoryMock = new Mock<IInterventionRepository>();
        var demandeInterventionRepositoryMock = new Mock<IDemandeInterventionRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        demandeInterventionRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(demande);

        technicienRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(technicien);

        var handler = new CreateInterventionCommandHandler(
            interventionRepositoryMock.Object,
            technicienRepositoryMock.Object,
            demandeInterventionRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateInterventionCommand(
            1,
            1,
            datePrevue
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("La demande doit être acceptée avant de planifier une intervention.", exception.Message);

        // Le technicien doit avoir été recherché
        technicienRepositoryMock.Verify(
            x => x.GetByIdAsync(
                1,
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        // L'intervention ne doit pas être créé
        interventionRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Intervention>(),
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