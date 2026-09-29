using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GestionInterventions.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GestionInterventions.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Technicien> Techniciens => Set<Technicien>();
    public DbSet<Equipement> Equipements => Set<Equipement>();
    public DbSet<DemandeIntervention> DemandeInterventions => Set<DemandeIntervention>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Intervention>()
        .OwnsOne(i => i.CompteRendu);
    }
}
