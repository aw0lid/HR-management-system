using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;

namespace Infrastructure.Cinfigrations
{
    internal class JobGradeConfig : IEntityTypeConfiguration<JobGrade>
    {
        public void Configure(EntityTypeBuilder<JobGrade> builder)
        {
            builder.ToTable("JobGrades");
            builder.HasKey(g => g.JobGradeId);

            builder.Property(g => g.JobGradeId).ValueGeneratedOnAdd();
           
            builder.Property(g => g.JobGradeName)
                   .HasMaxLength(50)
                   .IsRequired();

            
            builder.HasIndex(g => g.JobGradeName).IsUnique();

            builder.Property(g => g.GradeCode)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.HasIndex(g => g.Weight).IsUnique();

            
            builder.HasIndex(g => g.GradeCode).IsUnique();

            builder.Property(g => g.LevelDescription)
                   .HasMaxLength(255)
                   .IsRequired(false);
        }
    }
}
