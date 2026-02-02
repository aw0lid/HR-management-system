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
            builder.Property(u => u.UserId).ValueGeneratedOnAdd();

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(u => u.UserName)
                .IsUnique();

           builder.Property(u => u.UserPasswordHashing)
                .HasConversion(
                    p => p.Value,
                    v => new Password(v)
                )
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.PermissionsMask)
                .IsRequired()
                .HasColumnType("BIGINT")
                .HasDefaultValue(0);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(u => u.Employee)
                .WithOne() 
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Metadata.FindNavigation(nameof(User.UserLogs))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}