namespace GestionInterventions.Application.Common.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime Expiration) GenerateToken(
        string userId,
        string email,
        string role,
        string nom,
        int? clientId = null,
        int? technicienId = null);
}