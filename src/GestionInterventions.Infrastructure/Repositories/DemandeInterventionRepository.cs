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

    public async Task<(List<DemandeIntervention> Items, int TotalCount)> GetByClientIdAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.DemandeInterventions
            .Where(d => d.Equipement.ClientId == clientId)
            .Include(d => d.Equipement)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.DateDemande)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<DemandeIntervention> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.DemandeInterventions
            .Include(d => d.Equipement)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.DateDemande)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
