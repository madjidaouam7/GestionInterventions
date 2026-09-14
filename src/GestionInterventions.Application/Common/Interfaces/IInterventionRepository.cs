using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.Common.Interfaces;

public interface IInterventionRepository
{
    Task AddAsync(Intervention Intervention, CancellationToken cancellationToken = default);
    Task<Intervention?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    //Task<List<DemandeIntervention>> GetEnAttenteAsync(CancellationToken cancellationToken = default);
    Task<List<Intervention>> GetByTechnicienIdAsync(int technicienId, CancellationToken cancellationToken = default);
    Task<List<Intervention>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Intervention?> GetByDemandeIdAsync(int demandeId, CancellationToken cancellationToken = default);
}
