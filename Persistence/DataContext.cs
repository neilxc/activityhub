using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>()
                .HasKey(k => new {k.ActivityId, k.AppUserId});

            builder.Entity<Value>()
                .HasData(
                    new Value{Id = 1, Name = "Value 101"},
                    new Value{Id = 2, Name = "Value 102"},
                    new Value{Id = 3, Name = "Value 103"}
                );
        }
    }
}