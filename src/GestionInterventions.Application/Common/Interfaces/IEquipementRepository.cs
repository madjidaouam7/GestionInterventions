using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IEquipementRepository
{
    Task AddAsync(Equipement equipement, CancellationToken cancellationToken = default);
    Task<Equipement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Equipement> Items, int TotalCount)> GetByClientIdAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Equipement?> GetHistoriqueAsync(int equipementId, CancellationToken cancellationToken = default);
    Task<List<Equipement>> GetAllAsync(CancellationToken cancellationToken = default);
}
