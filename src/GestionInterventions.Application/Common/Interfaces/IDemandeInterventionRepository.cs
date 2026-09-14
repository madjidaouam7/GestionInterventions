using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IDemandeInterventionRepository
{
    Task AddAsync(DemandeIntervention demandeIntervention, CancellationToken cancellationToken = default);
    Task<DemandeIntervention?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<DemandeIntervention>> GetEnAttenteAsync(CancellationToken cancellationToken = default);
    Task<List<DemandeIntervention>> GetByClientIdAsync(int clientId , CancellationToken cancellationToken = default);
    Task<List<DemandeIntervention>> GetAllAsync(CancellationToken cancellationToken = default);
}

