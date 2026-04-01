using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Infraestructure.Context.Configurations
{
    public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
    {
        public void Configure(EntityTypeBuilder<Destination> entity)
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(d => d.Description)
                  .HasMaxLength(2000);

            entity.Property(d => d.Country)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(d => d.ImageUrl)
                  .HasMaxLength(500);

            entity.Property(d => d.Category)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(d => d.CreatedBy)
                  .HasMaxLength(450)
                  .IsRequired();

            entity.Property(d => d.CreatedAt)
                  .HasColumnType("timestamptz");

            entity.Property(d => d.UpdatedAt)
                  .HasColumnType("timestamptz");
        }
    }
}
