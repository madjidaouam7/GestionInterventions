using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionInterventions.Infrastructure.Repositories;

public class EquipementRepository : IEquipementRepository
{
    private readonly ApplicationDbContext _context;

    public EquipementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Equipement equipement, CancellationToken cancellationToken)
    {
        await _context.Equipements.AddAsync(equipement, cancellationToken);
    }

    public async Task<Equipement?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Equipements.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<(List<Equipement> Items, int TotalCount)> GetByClientIdAsync(int clientId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Equipements
            .Where(e => e.ClientId == clientId)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Equipement?> GetHistoriqueAsync(int equipementId, CancellationToken cancellationToken)
    {
        return await _context.Equipements.Include(e => e.Demandes)
                                         .ThenInclude(d => d.Interventions)
                                         .ThenInclude(i => i.CompteRendu)
                                         .FirstOrDefaultAsync(e => e.Id == equipementId, cancellationToken);
    }


    public async Task<List<Equipement>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Equipements.ToListAsync(cancellationToken);
    }
}
