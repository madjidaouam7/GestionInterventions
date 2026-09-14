namespace GestionInterventions.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }
    int? ClientId { get; }
    int? TechnicienId { get; }
}