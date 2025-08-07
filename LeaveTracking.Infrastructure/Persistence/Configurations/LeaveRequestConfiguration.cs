using LeaveTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveTracking.Infrastructure.Persistence.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StartDate)
                .IsRequired();
            builder.Property(x => x.EndDate)
                .IsRequired();
            builder.Property(x => x.LeaveType)
                .IsRequired()
                .HasConversion<string>();
            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(500);
            builder.Property(x => x.Status)
                .IsRequired();
            builder.Property(x => x.RejectedReason)
                .HasMaxLength(500)
                .IsRequired(false);
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}