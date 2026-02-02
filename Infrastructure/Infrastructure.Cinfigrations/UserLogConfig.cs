using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
    {
        public void Configure(EntityTypeBuilder<UserLog> builder)
        {
            builder.ToTable("UserLogs");

            builder.HasKey(l => l.LogId);
            builder.Property(l => l.LogId).ValueGeneratedOnAdd();

            builder.Property(l => l.LogTime)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(l => l.User)
                .WithMany(u => u.UserLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}