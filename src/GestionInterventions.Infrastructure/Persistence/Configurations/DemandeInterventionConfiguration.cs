using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionInterventions.Infrastructure.Persistence.Configurations;

public class DemandeInterventionConfiguration : IEntityTypeConfiguration<DemandeIntervention>
{
    public void Configure(EntityTypeBuilder<DemandeIntervention> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.DateDemande)
            .IsRequired();

        builder.Property(d => d.Statut)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.Priorite)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(d => d.Equipement)
            .WithMany(e => e.Demandes)
            .HasForeignKey(d => d.EquipementId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
