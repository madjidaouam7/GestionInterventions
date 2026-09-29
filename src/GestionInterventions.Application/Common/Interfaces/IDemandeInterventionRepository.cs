using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IDemandeInterventionRepository
{
    Task AddAsync(DemandeIntervention demandeIntervention, CancellationToken cancellationToken = default);
    Task<DemandeIntervention?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DemandeIntervention>> GetEnAttenteAsync(CancellationToken cancellationToken = default);
    Task<(List<DemandeIntervention> Items, int TotalCount)> GetByClientIdAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<DemandeIntervention> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}

