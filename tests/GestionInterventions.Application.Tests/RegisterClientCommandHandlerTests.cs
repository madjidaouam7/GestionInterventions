using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Clients.Commands.RegisterClient;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class RegisterClientCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandCreationIdentityReussit_CreeClientEtSauvegarde()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "client1@gmail.com",
                "Password123!",
                "Client"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        var handler = new RegisterClientCommandHandler(
            identityServiceMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RegisterClientCommand(
            "client1@gmail.com",
            "Password123!",
            "Client 1",
            "0123456789",
            "Alger"
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        clientRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Client>(c =>
                    c.Nom == "Client 1" &&
                    c.Email == "client1@gmail.com" &&
                    c.Telephone == "0123456789" &&
                    c.Adresse == "Alger" &&
                    c.IdentityUserId == "identity-user-123"
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        identityServiceMock.Verify(
            x => x.CreateUserAsync(
                "client1@gmail.com",
                "Password123!",
                "Client"
            ),
            Times.Once
        );

        identityServiceMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<string>()),
            Times.Never
        );

        // L'ID reste sa valeur par défaut dans un test unitaire sans EF Core.
        Assert.Equal(0, result);
    }


    [Fact]
    public async Task Handle_QuandCreationIdentityEchoue_LeverUneException()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "client1@gmail.com",
                "Password123!",
                "Client"
            ))
            .ReturnsAsync((false, (string?)null, new[] { "Le mot de passe est invalide.", "L'email existe déjà." }));

        var handler = new RegisterClientCommandHandler(
            identityServiceMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RegisterClientCommand(
            "client1@gmail.com",
            "Password123!",
            "Client 1",
            "0123456789",
            "Alger"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Le mot de passe est invalide. L'email existe déjà.",
            exception.Message
        );

        clientRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Client>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );

        identityServiceMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<string>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandDonneesClientSontInvalides_SupprimeUtilisateurEtRelanceException()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "client1@gmail.com",
                "Password123!",
                "Client"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        var handler = new RegisterClientCommandHandler(
            identityServiceMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RegisterClientCommand(
            "client1@gmail.com",
            "Password123!",
            "",
            "0123456789",
            "Alger"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Le nom du client est obligatoire.",
            exception.Message
        );

        clientRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Client>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );

        identityServiceMock.Verify(
            x => x.DeleteUserAsync("identity-user-123"),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandSauvegardeEchoue_SupprimeUtilisateurEtRelanceException()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "client1@gmail.com",
                "Password123!",
                "Client"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erreur de sauvegarde."));

        var handler = new RegisterClientCommandHandler(
            identityServiceMock.Object,
            clientRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new RegisterClientCommand(
            "client1@gmail.com",
            "Password123!",
            "Client 1",
            "0123456789",
            "Alger"
        );

        // Act et Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal("Erreur de sauvegarde.", exception.Message);

        clientRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Client>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );

        identityServiceMock.Verify(
            x => x.DeleteUserAsync("identity-user-123"),
            Times.Once
        );
    }
}