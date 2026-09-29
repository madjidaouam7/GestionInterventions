using GestionInterventions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionInterventions.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.IdentityUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(n => n.DateCreation)
            .IsRequired();

        builder.Property(n => n.EstLue)
            .IsRequired();

        builder.HasIndex(n => new
        {
            n.IdentityUserId,
            n.EstLue
        });

        builder.HasIndex(n => n.DateCreation);
    }
}