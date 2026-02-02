using Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AdminActionConfiguration : IEntityTypeConfiguration<PendingAdminAction>
    {
        public void Configure(EntityTypeBuilder<PendingAdminAction> builder)
        {
            builder.ToTable("PendingAdminActions");

            builder.HasKey(e => e.ActionId);
            builder.Property(e => e.ActionId).ValueGeneratedOnAdd();


            builder.HasOne(e => e.Requestor)
                   .WithMany()
                   .HasForeignKey(e => e.RequestedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Processor)
                   .WithMany()
                   .HasForeignKey(e => e.ProcessedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.TargetUser)
                   .WithMany()
                   .HasForeignKey(e => e.TargetUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}