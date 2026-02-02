using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;

namespace Infrastructure.Cinfigrations
{
    internal class EmployeePhoneConfig : IEntityTypeConfiguration<EmployeePhone>
    {
        public void Configure(EntityTypeBuilder<EmployeePhone> builder)
        {
            builder.ToTable("EmployeePhones");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();


            builder.ComplexProperty(p => p.Value, pn =>
            {
                pn.Property(p => p.Value)
                  .HasColumnName("PhoneNumber")
                  .HasMaxLength(20)
                  .IsRequired();
            });


           

            builder.Property(p => p.IsPrimary).HasDefaultValue(false);
            builder.Property(p => p.IsActive).HasDefaultValue(true);

            builder.HasIndex(p => new { p.EmployeeId, p.IsPrimary })
                   .HasFilter("[IsPrimary] = 1")
                   .IsUnique();

            builder.HasOne<Employee>()
                   .WithMany(e => e.Phones)
                   .HasForeignKey(p => p.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class EmployeeEmailConfig : IEntityTypeConfiguration<EmployeeEmail>
    {
        public void Configure(EntityTypeBuilder<EmployeeEmail> builder)
        {
            builder.ToTable("EmployeeEmails");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.ComplexProperty(e => e.Value, em =>
            {
                em.Property(p => p.Value)
                  .HasColumnName("Email")
                  .HasMaxLength(150)
                  .IsRequired();
            });

            

            builder.Property(e => e.IsPrimary).HasDefaultValue(false);
            builder.Property(e => e.IsActive).HasDefaultValue(true);

            builder.HasIndex(e => new { e.EmployeeId, e.IsPrimary })
                   .HasFilter("[IsPrimary] = 1")
                   .IsUnique();

            builder.HasOne<Employee>()
                   .WithMany(e => e.Emails)
                   .HasForeignKey(e => e.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}