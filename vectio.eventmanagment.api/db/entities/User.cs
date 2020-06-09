using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace vectio.eventmanagement.api.db.entities
{
    public class User : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Firstname).HasMaxLength(50);
            builder.Property(x => x.Lastname).HasMaxLength(100);
        }
    }

    public class JsonQuery
    {
        public dynamic JsonData { get; set; }
    }

    public class JsonQueryConfig : IEntityTypeConfiguration<JsonQuery>
    {
        public void Configure(EntityTypeBuilder<JsonQuery> builder)
        {
            builder.HasNoKey();
            builder.Property(x => x.JsonData).HasJsonConversion();
        }
    }

}
