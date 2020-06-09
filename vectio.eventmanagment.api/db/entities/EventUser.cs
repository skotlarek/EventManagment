using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace vectio.eventmanagement.api.db.entities
{
    public class EventUser
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }

    public class EventUserConfig : IEntityTypeConfiguration<EventUser>
    {
        public void Configure(EntityTypeBuilder<EventUser> builder)
        {
            builder.HasKey(x => x.Id).IsClustered();
            builder.Property(X => X.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(x => x.Firstname).HasMaxLength(50);
            builder.Property(x => x.Lastname).HasMaxLength(100);
            builder.Property(x => x.CompanyName).HasMaxLength(200);
            builder.Property(x => x.Email).HasMaxLength(100);
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.HasOne<Event>().WithMany(x => x.EventUsers).HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
