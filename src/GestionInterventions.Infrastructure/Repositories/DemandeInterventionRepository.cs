using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Infrastructure.Repositories;

public class DemandeInterventionRepository : IDemandeInterventionRepository
{
    private readonly ApplicationDbContext _context;

    public DemandeInterventionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DemandeIntervention demandeIntervention, CancellationToken cancellationToken)
    {
        await _context.DemandeInterventions.AddAsync(demandeIntervention, cancellationToken);
    }

    public async Task<DemandeIntervention?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.DemandeInterventions.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<List<DemandeIntervention>> GetEnAttenteAsync(CancellationToken cancellationToken)
    {
        return await _context.DemandeInterventions.Where(d => d.Statut == StatutDemande.EnAttente)
                                            .ToListAsync(cancellationToken);
    }

    public async Task<List<DemandeIntervention>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken)
    {
        return await _context.DemandeInterventions.Where(d => d.Equipement.ClientId == clientId)
                                         .Include(d => d.Equipement)
                                         .ToListAsync(cancellationToken);
    }

    public async Task<List<DemandeIntervention>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.DemandeInterventions
            .Include(d => d.Equipement)
            .ToListAsync(cancellationToken);
    }
}