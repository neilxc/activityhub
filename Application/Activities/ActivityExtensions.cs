using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Activities
{
    public static class ActivityExtensions
    {
        public static IQueryable<Activity> GetAllData(this DbSet<Activity> activities)
        {
            return activities
                .Include(x => x.GeoCoordinate)
                .Include(x => x.Attendees)
                .ThenInclude(x => x.AppUser)
                .AsNoTracking();
        }
    }
}