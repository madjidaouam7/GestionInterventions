using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Features.Auth.Commands.Login;
using GestionInterventions.Domain.Entities;
using Moq;

namespace GestionInterventions.Application.Tests;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_QuandIdentifiantsSontInvalides_LeverUneException()
    {
        // Arrange
        var identityServiceMock = new Mock<IIdentityService>();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();

        identityServiceMock
            .Setup(x => x.LoginAsync("client@gmail.com", "MotDePasse"))
            .ReturnsAsync((false, (string?)null, (string?)null, (string?)null));

        var handler = new LoginCommandHandler(
            identityServiceMock.Object,
            jwtTokenServiceMock.Object,
            clientRepositoryMock.Object,
            technicienRepositoryMock.Object
        );

        var command = new LoginCommand("client@gmail.com", "MotDePasse");

        // Act et Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );

        Assert.Equal(
            "Email ou mot de passe incorrect.",
            exception.Message
        );

        jwtTokenServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            ),
            Times.Never
        );

        clientRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        technicienRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandClientExiste_GenereTokenAvecClientIdEtNom()
    {
        // Arrange
        var client = new Client(
            "Client 1",
            "client@gmail.com",
            "0123456789",
            "Alger",
            "identity-client-123"
        );

        var idProperty = typeof(Client)
            .GetProperty(nameof(Client.Id))!;
        idProperty.SetValue(client, 1);

        var expiration = DateTime.UtcNow.AddHours(1);
        var identityServiceMock = new Mock<IIdentityService>();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();

        identityServiceMock
            .Setup(x => x.LoginAsync("client@gmail.com", "MotDePasse"))
            .ReturnsAsync((true, "identity-client-123", "client@gmail.com", "Client"));

        clientRepositoryMock
            .Setup(x => x.GetByIdentityUserIdAsync(
                "identity-client-123",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(client);

        jwtTokenServiceMock
            .Setup(x => x.GenerateToken(
                "identity-client-123",
                "client@gmail.com",
                "Client",
                "Client 1",
                1,
                null
            ))
            .Returns(("client-token", expiration));

        var handler = new LoginCommandHandler(
            identityServiceMock.Object,
            jwtTokenServiceMock.Object,
            clientRepositoryMock.Object,
            technicienRepositoryMock.Object
        );

        var command = new LoginCommand("client@gmail.com", "MotDePasse");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("client-token", result.Token);
        Assert.Equal(expiration, result.Expiration);

        clientRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                "identity-client-123",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        technicienRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        jwtTokenServiceMock.Verify(
            x => x.GenerateToken(
                "identity-client-123",
                "client@gmail.com",
                "Client",
                "Client 1",
                1,
                null
            ),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandClientMetierNExistePas_GenereTokenAvecEmailCommeNom()
    {
        // Arrange
        var expiration = DateTime.UtcNow.AddHours(1);
        var identityServiceMock = new Mock<IIdentityService>();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();

        identityServiceMock
            .Setup(x => x.LoginAsync("client@gmail.com", "MotDePasse"))
            .ReturnsAsync((true, "identity-client-123", "client@gmail.com", "Client"));

        clientRepositoryMock
            .Setup(x => x.GetByIdentityUserIdAsync(
                "identity-client-123",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync((Client?)null);

        jwtTokenServiceMock
            .Setup(x => x.GenerateToken(
                "identity-client-123",
                "client@gmail.com",
                "Client",
                "client@gmail.com",
                null,
                null
            ))
            .Returns(("client-token", expiration));

        var handler = new LoginCommandHandler(
            identityServiceMock.Object,
            jwtTokenServiceMock.Object,
            clientRepositoryMock.Object,
            technicienRepositoryMock.Object
        );

        var command = new LoginCommand("client@gmail.com", "MotDePasse");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("client-token", result.Token);
        Assert.Equal(expiration, result.Expiration);

        jwtTokenServiceMock.Verify(
            x => x.GenerateToken(
                "identity-client-123",
                "client@gmail.com",
                "Client",
                "client@gmail.com",
                null,
                null
            ),
            Times.Once
        );

        technicienRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
    }


    [Fact]
    public async Task Handle_QuandTechnicienExiste_GenereTokenAvecTechnicienIdEtNom()
    {
        // Arrange
        var technicien = new Technicien(
            "Technicien 1",
            "technicien@gmail.com",
            "0123456789",
            "Alger",
            "identity-technicien-123"
        );

        var idProperty = typeof(Technicien)
            .GetProperty(nameof(Technicien.Id))!;
        idProperty.SetValue(technicien, 2);

        var expiration = DateTime.UtcNow.AddHours(1);
        var identityServiceMock = new Mock<IIdentityService>();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();

        identityServiceMock
            .Setup(x => x.LoginAsync("technicien@gmail.com", "MotDePasse"))
            .ReturnsAsync((true, "identity-technicien-123", "technicien@gmail.com", "Technicien"));

        technicienRepositoryMock
            .Setup(x => x.GetByIdentityUserIdAsync(
                "identity-technicien-123",
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(technicien);

        jwtTokenServiceMock
            .Setup(x => x.GenerateToken(
                "identity-technicien-123",
                "technicien@gmail.com",
                "Technicien",
                "Technicien 1",
                null,
                2
            ))
            .Returns(("technicien-token", expiration));

        var handler = new LoginCommandHandler(
            identityServiceMock.Object,
            jwtTokenServiceMock.Object,
            clientRepositoryMock.Object,
            technicienRepositoryMock.Object
        );

        var command = new LoginCommand("technicien@gmail.com", "MotDePasse");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("technicien-token", result.Token);
        Assert.Equal(expiration, result.Expiration);

        clientRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        technicienRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                "identity-technicien-123",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        jwtTokenServiceMock.Verify(
            x => x.GenerateToken(
                "identity-technicien-123",
                "technicien@gmail.com",
                "Technicien",
                "Technicien 1",
                null,
                2
            ),
            Times.Once
        );
    }


    [Fact]
    public async Task Handle_QuandRoleEstResponsable_GenereTokenResponsable()
    {
        // Arrange
        var expiration = DateTime.UtcNow.AddHours(1);
        var identityServiceMock = new Mock<IIdentityService>();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var clientRepositoryMock = new Mock<IClientRepository>();
        var technicienRepositoryMock = new Mock<ITechnicienRepository>();

        identityServiceMock
            .Setup(x => x.LoginAsync("responsable@gmail.com", "MotDePasse"))
            .ReturnsAsync((true, "identity-responsable-123", "responsable@gmail.com", "Responsable"));

        jwtTokenServiceMock
            .Setup(x => x.GenerateToken(
                "identity-responsable-123",
                "responsable@gmail.com",
                "Responsable",
                "Responsable",
                null,
                null
            ))
            .Returns(("responsable-token", expiration));

        var handler = new LoginCommandHandler(
            identityServiceMock.Object,
            jwtTokenServiceMock.Object,
            clientRepositoryMock.Object,
            technicienRepositoryMock.Object
        );

        var command = new LoginCommand("responsable@gmail.com", "MotDePasse");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("responsable-token", result.Token);
        Assert.Equal(expiration, result.Expiration);

        clientRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        technicienRepositoryMock.Verify(
            x => x.GetByIdentityUserIdAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        jwtTokenServiceMock.Verify(
            x => x.GenerateToken(
                "identity-responsable-123",
                "responsable@gmail.com",
                "Responsable",
                "Responsable",
                null,
                null
            ),
            Times.Once
        );
    }
}