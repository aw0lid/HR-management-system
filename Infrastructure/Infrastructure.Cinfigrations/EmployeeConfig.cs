using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;

namespace Infrastructure.Cinfigrations
{
    internal class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.EmployeeId);

            builder.Property(e => e.EmployeeId).ValueGeneratedOnAdd();
            
            builder.Property(e => e.EmployeeFirstName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.EmployeeSecondName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.EmployeeThirdName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(e => e.EmployeeLastName)
                   .HasMaxLength(50)
                   .IsRequired();

            
            builder.Property(e => e.EmployeeCode)
                   .HasMaxLength(20)
                   .IsRequired();
            builder.HasIndex(e => e.EmployeeCode).IsUnique();


            builder.ComplexProperty(e => e.EmployeeNationalNumber, n =>
            {
                n.Property(vo => vo.Value)
                 .HasColumnName("NationalNumber")
                 .HasMaxLength(14)
                 .IsRequired();
            });

           
          


            builder.Property(e => e.EmployeeGender)
                   .HasColumnType("tinyint")
                   .IsRequired();


            builder.ComplexProperty(e => e.EmployeeBirthDate, propBuilder =>
            {
                propBuilder.Property(b => b.Value)
                    .HasColumnName("BirthDate") 
                    .HasColumnType("date")
                    .IsRequired();
            });


            builder.Property(e => e.IsForeign)
                   .HasDefaultValue(false);

            builder.Property(w => w.HireDate)
                  .HasColumnType("date")
                  .IsRequired();

            builder.Property(w => w.EmployeeResignationDate)
                   .HasColumnType("date")
                   .IsRequired(false);

            builder.Property(e => e.EmployeeCurrentAddress)
                     .HasMaxLength(255)
                     .IsRequired();

            builder.Property(w => w.Status)
                   .HasColumnType("tinyint")
                   .IsRequired();


            builder.HasOne(e => e.Nationality)
                   .WithMany()
                   .HasForeignKey(e => e.EmployeeNationalityId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.Navigation(e => e.WorkInfoHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(e => e.Phones).UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Navigation(e => e.Emails).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(e => e.CurrentWorkInfo);
        }
    }
}
