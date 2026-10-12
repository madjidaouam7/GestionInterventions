using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionInterventions.Infrastructure.Persistence.Configurations;

public class EquipementPhotoConfiguration : IEntityTypeConfiguration<EquipementPhoto>
{
    public void Configure(EntityTypeBuilder<EquipementPhoto> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NomFichier)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.CheminFichier)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.DateAjout)
            .IsRequired();

        builder.HasOne(p => p.Equipement)
            .WithMany(e => e.Photos)
            .HasForeignKey(p => p.EquipementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}