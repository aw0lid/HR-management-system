using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;


namespace Infrastructure.Cinfigrations
{
    internal class JobTitleConfig : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.ToTable("JobTitles");
            builder.HasKey(t => t.JobTitleId);

            builder.Property(t => t.JobTitleId).ValueGeneratedOnAdd();
           
            builder.Property(t => t.JobTitleName)
                   .HasMaxLength(100)
                   .IsRequired();

            
            builder.HasIndex(t => t.JobTitleName).IsUnique();

            builder.Property(t => t.JobTitleCode)
                   .HasMaxLength(10)
                   .IsRequired();

            
            builder.HasIndex(t => t.JobTitleCode).IsUnique();

            builder.Property(t => t.JobTitleDescription)
                   .HasMaxLength(255)
                   .IsRequired(false);
        }
    }
}
