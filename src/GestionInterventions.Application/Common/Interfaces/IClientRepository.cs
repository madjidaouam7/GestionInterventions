using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IClientRepository
{
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
}
