using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace vectio.eventmanagement.api.db.entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public DateTime EventDate { get; set; }
        public int? DurationTime { get; set; }
        public string EventName { get; set; }
        public int? NumberSeats { get; set; }
        public bool LimitedPlaces { get; set; }
        public ICollection<EventUser> EventUsers { get; set; }
    }

    public class EventConfig : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {

            builder.HasKey(x => x.Id).IsClustered();
            builder.Property(X => X.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(x => x.Content).HasColumnType("nvarchar(max)");
        }
    }

}
