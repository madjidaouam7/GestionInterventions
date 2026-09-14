using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionInterventions.Infrastructure.Persistence.Configurations;

public class EquipementConfiguration : IEntityTypeConfiguration<Equipement>
{
    public void Configure(EntityTypeBuilder<Equipement> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nom)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NumeroSerie)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Localisation)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.Statut)
            .IsRequired()
            .HasConversion<string>(); // stocke l'enum en texte lisible plutôt qu'en int

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Demandes)
            .WithOne(d => d.Equipement)
            .HasForeignKey(d => d.EquipementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}