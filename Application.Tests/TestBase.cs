using System;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Tests
{
    public class TestBase
    {
        public DataContext GetDbContext(bool useSqLite = false)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            if (useSqLite)
            {
                builder.UseSqlite("DataSource=:memory:", x => { });
            }
            else
            {
                builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            }
            
            var dbContext = new DataContext(builder.Options);
            if (useSqLite)
            {
                // SQLite needs to open connection to db.
                // Not required for in memory database
                dbContext.Database.OpenConnection();
            }

            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}