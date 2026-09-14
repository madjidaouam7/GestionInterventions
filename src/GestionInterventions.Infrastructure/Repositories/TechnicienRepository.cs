using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionInterventions.Infrastructure.Repositories;

public class TechnicienRepository : ITechnicienRepository
{
    private readonly ApplicationDbContext _context;

    public TechnicienRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Technicien technicien, CancellationToken cancellationToken)
    {
        await _context.Techniciens.AddAsync(technicien, cancellationToken);
    }

    public async Task<Technicien?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Techniciens.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Technicien>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Techniciens.ToListAsync(cancellationToken);
    }

    public async Task<Technicien?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken)
{
    return await _context.Techniciens.FirstOrDefaultAsync(t => t.IdentityUserId == identityUserId, cancellationToken);
}
}
