using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Tokens");
            builder.HasKey(t => t.TokenId);

            builder.Property(t => t.value)
                .IsRequired()
                .HasColumnType("varchar(500)");

            builder.Property(t => t.Type).HasConversion<byte>();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

           builder.HasOne(t => t.ReplacedByToken)
                .WithMany()
                .HasForeignKey(t => t.ReplacedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}