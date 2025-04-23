using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelingApp.Domain.Entities;

namespace TravelingApp.Infraestructure.Context.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.Property(u => u.IsActive)
                  .HasColumnType("boolean")
                  .HasDefaultValue(false);

            entity.Property(u => u.UpdatedAt)
                  .HasColumnType("timestamptz");

            entity.Property(u => u.Version)
                  .HasColumnName("xmin")
                  .HasColumnType("xid")
                  .ValueGeneratedOnAddOrUpdate()
                  .IsConcurrencyToken();
        }
    }
}
