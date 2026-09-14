using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IEquipementRepository
{
    Task AddAsync(Equipement equipement, CancellationToken cancellationToken = default);
    Task<Equipement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Equipement>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<Equipement?> GetHistoriqueAsync(int equipementId, CancellationToken cancellationToken = default);
    Task<List<Equipement>> GetAllAsync(CancellationToken cancellationToken = default);
}
