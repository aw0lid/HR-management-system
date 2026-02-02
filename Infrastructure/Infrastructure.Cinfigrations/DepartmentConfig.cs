using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Cinfigrations
{
    internal class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.DepartmentId).ValueGeneratedOnAdd();
           
            builder.Property(d => d.DepartmentName)
                   .HasMaxLength(50)
                   .IsRequired();

            
            builder.HasIndex(d => d.DepartmentName).IsUnique();

            builder.Property(d => d.DepartmentCode)
                   .HasMaxLength(10)
                   .IsRequired();

            
            builder.HasIndex(d => d.DepartmentCode).IsUnique();

            builder.Property(d => d.DepartmentDescription)
                   .HasMaxLength(255)
                   .IsRequired(false);
        }
    }
}
