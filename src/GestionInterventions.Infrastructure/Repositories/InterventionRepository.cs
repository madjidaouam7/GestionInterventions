using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Infrastructure.Repositories;

public class InterventionRepository : IInterventionRepository
{
    private readonly ApplicationDbContext _context;

    public InterventionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Intervention Intervention, CancellationToken cancellationToken)
    {
        await _context.Interventions.AddAsync(Intervention, cancellationToken);
    }

    public async Task<Intervention?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Interventions.Include(i => i.Demande)
                                           .ThenInclude(d => d.Equipement)
                                           .Include(i => i.Technicien)
                                           .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    /*public async Task<List<DemandeIntervention>> GetEnAttenteAsync(CancellationToken cancellationToken)
    {
       return await _context.DemandeInterventions.Where(d => d.Statut == StatutDemande.EnAttente)
                                           .ToListAsync(cancellationToken);
    }*/

    public async Task<List<Intervention>> GetByTechnicienIdAsync(int technicienId, CancellationToken cancellationToken)
    {
        return await _context.Interventions.Where(i => i.TechnicienId == technicienId)
                                           .Include(i => i.Technicien)
                                           .ToListAsync(cancellationToken);
    }


    public async Task<List<Intervention>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Interventions
            .Include(i => i.Demande)
            .ThenInclude(d => d.Equipement)
            .Include(i => i.Technicien)
            .ToListAsync(cancellationToken);
    }


    public async Task<Intervention?> GetByDemandeIdAsync(int demandeId, CancellationToken cancellationToken)
    {
        return await _context.Interventions.FirstOrDefaultAsync(i => i.DemandeId == demandeId, cancellationToken);
    }

}
