using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;

namespace Infrastructure.Cinfigrations
{
    internal class JobTitleLevelConfig : IEntityTypeConfiguration<JobTitleLevel>
    {
        public void Configure(EntityTypeBuilder<JobTitleLevel> builder)
        {
            builder.ToTable("JobTitleLevels");
            builder.HasKey(jl => jl.JobTitleLevelId);

            builder.Property(jl => jl.JobTitleLevelId).ValueGeneratedOnAdd();

            
            builder.HasIndex(jl => new { jl.JobTitleId, jl.JobGradeId }).IsUnique();

            
            builder.HasOne(jl => jl.JobTitle)
                   .WithMany()
                   .HasForeignKey(jl => jl.JobTitleId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(jl => jl.JobGrade)
                   .WithMany()
                   .HasForeignKey(jl => jl.JobGradeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
