using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionInterventions.Infrastructure.Persistence.Configurations;

public class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.DatePrevue)
            .IsRequired();

        builder.Property(i => i.Statut)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(i => i.Demande)
            .WithMany(d => d.Interventions)
            .HasForeignKey(i => i.DemandeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Technicien)
            .WithMany()
            .HasForeignKey(i => i.TechnicienId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
