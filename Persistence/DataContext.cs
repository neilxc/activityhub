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
        public DbSet<FollowedPeople> FollowedPeople { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserInterest> UserInterests { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActivityAttendee>(b =>
                {
                   b.HasKey(k => new {k.ActivityId, k.AppUserId});
                   b.HasOne(a => a.Activity)
                        .WithMany(at => at.Attendees)
                        .HasForeignKey(a => a.ActivityId);
                });

            builder.Entity<Value>()
                .HasData(
                    new Value{Id = 1, Name = "Value 101"},
                    new Value{Id = 2, Name = "Value 102"},
                    new Value{Id = 3, Name = "Value 103"}
                );

            builder.Entity<FollowedPeople>(b => 
            {
                b.HasKey(t => new {t.ObserverId, t.TargetId});

                b.HasOne(pt => pt.Observer)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(pt => pt.ObserverId);

                b.HasOne(pt => pt.Target)
                    .WithMany(t => t.Following)
                    .HasForeignKey(pt => pt.TargetId);
            });

            builder.Entity<UserInterest>(b =>
            {
                b.HasKey(t => new {t.AppUserId, t.InterestId});
                b.HasOne(x => x.AppUser)
                    .WithMany(y => y.UserInterests)
                    .HasForeignKey(x => x.AppUserId);
            });
        }
    }
}