using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;


namespace Infrastructure.Cinfigrations
{
    internal class EmployeeWorkInfoConfig : IEntityTypeConfiguration<EmployeeWorkInfo>
    {
        public void Configure(EntityTypeBuilder<EmployeeWorkInfo> builder)
        {
            builder.ToTable("EmployeeWorkInfo");
            builder.HasKey(w => w.EmployeeWorkInfoId);

            builder.Property(w => w.EmployeeWorkInfoId).ValueGeneratedOnAdd();

           

            builder.Property(e => e.FromDate).HasDefaultValueSql("GETDATE()");

            builder.Property(w => w.ToDate)
                .HasColumnType("date")
                .IsRequired(false);

            builder.Property(w => w.IsCurrent)
                   .HasDefaultValue(true);


            builder.HasOne(w => w.Manager)
                   .WithMany()
                   .HasForeignKey(w => w.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

           
            builder.HasOne(w => w.Department)
                   .WithMany()
                   .HasForeignKey(w => w.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(w => w.JobTitleLevel)
                   .WithMany()
                   .HasForeignKey(w => w.JobTitleLevelId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasIndex(w => new { w.EmployeeId, w.IsCurrent })
                   .HasFilter("[IsCurrent] = 1")
                   .IsUnique()
                   .HasDatabaseName("IX_Employee_WorkInfo_Current_Unique");
        }
    }
}
