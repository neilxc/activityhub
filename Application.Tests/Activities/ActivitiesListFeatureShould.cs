using System.Threading;
using Application.Activities;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Xunit;

namespace Application.Tests.Activities
{
    public class ActivitiesListFeatureShould
    {
        [Fact]
        public void ReturnListOfAllActivities()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "Return_list_of_values")
                .Options;
            
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mockMapper.CreateMapper();

            using (var context = new DataContext(options))
            {
                context.Activities.Add(new Activity {Id = 1, Title = "Test Activity 1"});
                context.Activities.Add(new Activity {Id = 2, Title = "Test Activity 2"});
                context.SaveChanges();
            }
            
            // use clean instance of context to run the test
            using (var context = new DataContext(options))
            {
                
                var sut = new List.Handler(context, mapper);
                var result = sut.Handle(new List.Query(null, null, null, null), CancellationToken.None).Result;
                
                Assert.Equal(2, result.ActivityCount);
                Assert.Equal("Test Activity 1", result.Activities[0].Title);
            }
        }
    }
}