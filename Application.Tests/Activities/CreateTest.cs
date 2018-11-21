using System;
using System.Threading;
using Application.Activities;
using AutoMapper;
using Domain;
using Xunit;

namespace Application.Tests.Activities
{
    public class CreateTest : TestBase
    {
        private readonly IMapper _mapper;
        
        public CreateTest()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = mockMapper.CreateMapper();
        }
       
        [Fact]
        public void Should_create_activity()
        {
            var context = GetDbContext();

            var activityCommand = new Create.Command
            {
                Activity = new Create.ActivityData
                {
                    City = "London",
                    Date = DateTime.Today,
                    Description = "Description of event",
                    GeoCoordinate = new GeoCoordinate
                    {
                        Id = 1,
                        Latitude = 20,
                        Longitude = 40
                    },
                    Title = "Test Activity",
                    Venue = "Test Venue"
                }
            };
            
//            var sut = new Create.Handler(context, _mapper);
//            var result = sut.Handle(activityCommand, CancellationToken.None).Result;
//            
//            Assert.NotNull(result);
//            Assert.Equal("Test Activity", result.Title);
        }
    }
}