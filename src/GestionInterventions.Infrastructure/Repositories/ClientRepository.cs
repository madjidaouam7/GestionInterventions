using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionInterventions.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Client>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Clients.ToListAsync(cancellationToken);
    }

    public async Task<Client?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken)
{
    return await _context.Clients.FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId,cancellationToken);
}
}