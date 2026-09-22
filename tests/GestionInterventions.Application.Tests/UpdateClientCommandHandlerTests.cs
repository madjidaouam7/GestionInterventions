using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Clients.Commands.UpdateClient;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class UpdateClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandClientExisteEtUtilisateurEstAutorise_ModifieCoordonneesEtSauvegarde()
    {
        // Arrange
        var client = new Client(
            "Client 1",
            "ancien@gmail.com",
            "0123456789",
            "Alger",
            "identity-user-123"
        );

        var idProperty = typeof(Client)
            .GetProperty(nameof(Client.Id))!;
        idProperty.SetValue(client, 1);

        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Client");
        currentUserServiceMock
            .SetupGet(x => x.ClientId)
            .Returns(1);

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var handler = new UpdateClientCommandHandler(
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new UpdateClientCommand(
            1,
            "nouveau@gmail.com",
            "0987654321",
            "Oran"
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Client 1", result.Nom);
        Assert.Equal("nouveau@gmail.com", result.Email);
        Assert.Equal("0987654321", result.Telephone);
        Assert.Equal("Oran", result.Adresse);

        Assert.Equal("nouveau@gmail.com", client.Email);
        Assert.Equal("0987654321", client.Telephone);
        Assert.Equal("Oran", client.Adresse);

        clientRepositoryMock.Verify(
            x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()),
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
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        var handler = new UpdateClientCommandHandler(
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new UpdateClientCommand(
            1,
            "nouveau@gmail.com",
            "0987654321",
            "Oran"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Aucun client trouvé avec l'id 1.",
            exception.Message
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandClientModifieLeProfilDUnAutreClient_LeverUneException()
    {
        // Arrange
        var client = new Client(
            "Client 1",
            "client1@gmail.com",
            "0123456789",
            "Alger",
            "identity-user-123"
        );

        var idProperty = typeof(Client)
            .GetProperty(nameof(Client.Id))!;
        idProperty.SetValue(client, 1);

        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Client");
        currentUserServiceMock
            .SetupGet(x => x.ClientId)
            .Returns(2);

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var handler = new UpdateClientCommandHandler(
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new UpdateClientCommand(
            1,
            "nouveau@gmail.com",
            "0987654321",
            "Oran"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<ForbiddenAccessException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Vous ne pouvez modifier que votre propre profil.",
            exception.Message
        );

        Assert.Equal("client1@gmail.com", client.Email);
        Assert.Equal("0123456789", client.Telephone);
        Assert.Equal("Alger", client.Adresse);

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandCoordonneesSontInvalides_LeverUneException()
    {
        // Arrange
        var client = new Client(
            "Client 1",
            "client1@gmail.com",
            "0123456789",
            "Alger",
            "identity-user-123"
        );

        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();

        currentUserServiceMock
            .SetupGet(x => x.Role)
            .Returns("Responsable");

        clientRepositoryMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        var handler = new UpdateClientCommandHandler(
            clientRepositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object
        );

        var command = new UpdateClientCommand(
            1,
            "email-invalide",
            "0987654321",
            "Oran"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "L'email du client est invalide.",
            exception.Message
        );

        Assert.Equal("client1@gmail.com", client.Email);
        Assert.Equal("0123456789", client.Telephone);
        Assert.Equal("Alger", client.Adresse);

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}