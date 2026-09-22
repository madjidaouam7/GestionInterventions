using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Techniciens.Commands.CreateTechnicien;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using Moq;

namespace GestionInterventions.Application.Tests;

public class CreateTechnicienCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandCreationIdentityReussit_CreeTechnicienEtSauvegarde()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "tech1@gmail.com",
                "Password123!",
                "Technicien"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        var handler = new CreateTechnicienCommandHandler(
            identityServiceMock.Object,
            technicienRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateTechnicienCommand(
            "tech1@gmail.com",
            "Password123!",
            "Tech 1",
            "0123456789",
            "Alger"
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        technicienRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Technicien>(t =>
                    t.Nom == "Tech 1" &&
                    t.Email == "tech1@gmail.com" &&
                    t.Telephone == "0123456789" &&
                    t.Adresse == "Alger" &&
                    t.IdentityUserId == "identity-user-123"
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
                "tech1@gmail.com",
                "Password123!",
                "Technicien"
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
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "tech1@gmail.com",
                "Password123!",
                "Technicien"
            ))
            .ReturnsAsync((false, (string?)null, new[] { "Le mot de passe est invalide.", "L'email existe déjà." }));

        var handler = new CreateTechnicienCommandHandler(
            identityServiceMock.Object,
            technicienRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateTechnicienCommand(
            "tech1@gmail.com",
            "Password123!",
            "Tech 1",
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

        technicienRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Technicien>(),
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
    public async Task Handle_QuandDonneesTechnicienSontInvalides_SupprimeUtilisateurEtRelanceException()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "tech1@gmail.com",
                "Password123!",
                "Technicien"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        var handler = new CreateTechnicienCommandHandler(
            identityServiceMock.Object,
            technicienRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateTechnicienCommand(
            "tech1@gmail.com",
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
            "Le nom du technicien est obligatoire.",
            exception.Message
        );

        technicienRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Technicien>(),
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
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        identityServiceMock
            .Setup(x => x.CreateUserAsync(
                "tech1@gmail.com",
                "Password123!",
                "Technicien"
            ))
            .ReturnsAsync((true, "identity-user-123", Enumerable.Empty<string>()));

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Erreur de sauvegarde."));

        var handler = new CreateTechnicienCommandHandler(
            identityServiceMock.Object,
            technicienRepositoryMock.Object,
            unitOfWorkMock.Object
        );

        var command = new CreateTechnicienCommand(
            "tech1@gmail.com",
            "Password123!",
            "Tech 1",
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

        technicienRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<Technicien>(),
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