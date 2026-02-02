using Domain.Entites;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(u => u.UserName).IsUnique();

            builder.Property(u => u.UserPasswordHashing)
                .HasConversion(p => p!.Value, v => new Password(v))
                .HasColumnName("PasswordHash")
                .HasMaxLength(500);

            builder.Property(u => u.Role).HasConversion<byte>();
            builder.Property(u => u.status).HasConversion<byte>();

            builder.HasOne(u => u.Employee)
                .WithOne()
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(u => u.Tokens)
                .HasField("_tokens")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(u => u.UserLogs)
                .HasField("_userLogs")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}