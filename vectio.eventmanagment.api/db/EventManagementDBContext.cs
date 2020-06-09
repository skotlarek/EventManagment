using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using vectio.eventmanagement.api.db.entities;

namespace vectio.eventmanagement.api.db
{
    public class EventManagementDBContext : IdentityDbContext<User>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<JsonQuery> JsonQueries { get; set; }
        public EventManagementDBContext(DbContextOptions<EventManagementDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

        }
    }
}
