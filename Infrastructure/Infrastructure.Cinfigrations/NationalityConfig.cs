using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites;


namespace Infrastructure.Cinfigrations
{
    internal class NationalityConfig : IEntityTypeConfiguration<Nationality>
    {
        public void Configure(EntityTypeBuilder<Nationality> builder)
        {
            builder.ToTable("Nationalities");
            builder.HasKey(n => n.NationalityId);

            builder.Property(n => n.NationalityId).ValueGeneratedOnAdd();
            
            builder.Property(n => n.NationalityName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Ignore(n => n.IsLocal);
            
            builder.HasIndex(n => n.NationalityName).IsUnique();
        }
    }
}
