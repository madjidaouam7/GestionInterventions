using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IClientRepository _clientRepository;
    private readonly ITechnicienRepository _technicienRepository;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IClientRepository clientRepository,
        ITechnicienRepository technicienRepository)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _clientRepository = clientRepository;
        _technicienRepository = technicienRepository;
    }

    public async Task<LoginResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Vérifier Email + Password avec ASP.NET Identity
        var loginResult = await _identityService.LoginAsync(
            request.Email,
            request.Password);

        if (!loginResult.Succeeded)
        {
            throw new UnauthorizedAccessException(
                "Email ou mot de passe incorrect.");
        }

        int? clientId = null;
        int? technicienId = null;
        string nom = loginResult.Email!;

        // 2. Récupérer l'ID métier selon le rôle
        if (loginResult.Role == "Client")
        {
            var client = await _clientRepository.GetByIdentityUserIdAsync(
                loginResult.UserId!,
                cancellationToken);

            if (client is not null)
            {
                clientId = client.Id;
                nom = client.Nom;
            }
        }
        else if (loginResult.Role == "Technicien")
        {
            var technicien =
                await _technicienRepository.GetByIdentityUserIdAsync(
                    loginResult.UserId!,
                    cancellationToken);

            if (technicien is not null)
            {
                technicienId = technicien.Id;
                nom = technicien.Nom;
            }
        }
        else if (loginResult.Role == "Responsable")
        {
            nom = "Responsable";
        }

        // 3. Générer le JWT
        var (token, expiration) = _jwtTokenService.GenerateToken(
            loginResult.UserId!,
            loginResult.Email!,
            loginResult.Role!,
            nom,
            clientId,
            technicienId);

        // 4. Retourner la réponse
        return new LoginResponseDto
        {
            Token = token,
            Expiration = expiration
        };
    }
}