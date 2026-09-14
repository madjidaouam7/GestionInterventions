namespace GestionInterventions.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email, string password, string role);

    Task DeleteUserAsync(string userId);

    Task<(bool Succeeded, string? UserId, string? Email, string? Role)>
        LoginAsync(string email, string password);
}