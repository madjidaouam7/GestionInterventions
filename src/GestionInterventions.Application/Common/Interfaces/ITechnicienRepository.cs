using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface ITechnicienRepository
{
    Task AddAsync(Technicien technicien, CancellationToken cancellationToken = default);
    Task<Technicien?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Technicien>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Technicien?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
}
