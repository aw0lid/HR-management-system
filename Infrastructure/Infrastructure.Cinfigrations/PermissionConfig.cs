using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.PermissionId);
            builder.Property(p => p.PermissionId).ValueGeneratedOnAdd();

            builder.Property(p => p.PermissionName)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.BitValue)
                .IsRequired()
                .HasColumnType("BIGINT");
        }
    }
}