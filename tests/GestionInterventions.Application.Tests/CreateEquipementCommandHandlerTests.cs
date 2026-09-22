using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Equipements.Commands.CreateEquipement;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class CreateEquipementCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandClientExisteEtUtilisateurAutorise_CreeEquipementEtSauvegarde()
    {
        // Arrange
        var client = new Client(
            "Ahmed",
            "ahmed@example.com",
            "0550000000",
            "Alger",
            "identity-user-123"
        );

        var clientRepositoryMock = new Mock<IClientRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Responsable");

        var handler = new CreateEquipementCommandHandler(
            equipementRepositoryMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new CreateEquipementCommand(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        equipementRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Equipement>(e =>
                    e.Nom == "Ordinateur" &&
                    e.NumeroSerie == "SN12345" &&
                    e.Description == "Ordinateur de bureau" &&
                    e.Localisation == "Bureau 1" &&
                    e.ClientId == 1
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
    public async Task Handle_QuandClientNExistePas_LeverUneException()
    {
        // Arrange
        var clientRepositoryMock = new Mock<IClientRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var handler = new CreateEquipementCommandHandler(
            equipementRepositoryMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new CreateEquipementCommand(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("Le client n'existe pas.", exception.Message);

        // L'équipement ne doit pas être créé
        equipementRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Equipement>(),
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
    public async Task Handle_QuandClientEssaieDeCreerEquipementPourUnAutreClient_LeverUneException()
    {
        // Arrange
        var client = new Client(
            "Ahmed",
            "ahmed@example.com",
            "0550000000",
            "Alger",
            "identity-user-123"
        );

        var clientRepositoryMock = new Mock<IClientRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Client");

        currentUserServiceMock
            .SetupGet(x => x.ClientId)
            .Returns(1);

        var handler = new CreateEquipementCommandHandler(
            equipementRepositoryMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new CreateEquipementCommand(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            2
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<ForbiddenAccessException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("Vous ne pouvez créer un équipement que pour votre propre compte.", exception.Message);

        // L'équipement ne doit pas être créé
        equipementRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Equipement>(),
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
    public async Task Handle_QuandClientCreeEquipementPourSonPropreCompte_CreeEquipementEtSauvegarde()
    {
        // Arrange
        var client = new Client(
            "Ahmed",
            "ahmed@example.com",
            "0550000000",
            "Alger",
            "identity-user-123"
        );

        var clientRepositoryMock = new Mock<IClientRepository>();
        var equipementRepositoryMock = new Mock<IEquipementRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Client");

        currentUserServiceMock
            .SetupGet(x => x.ClientId)
            .Returns(1);

        var handler = new CreateEquipementCommandHandler(
            equipementRepositoryMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new CreateEquipementCommand(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        equipementRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Equipement>(e =>
                    e.Nom == "Ordinateur" &&
                    e.NumeroSerie == "SN12345" &&
                    e.Description == "Ordinateur de bureau" &&
                    e.Localisation == "Bureau 1" &&
                    e.ClientId == 1
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
}