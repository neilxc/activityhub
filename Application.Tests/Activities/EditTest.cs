using System.Threading;
using Application.Activities;
using AutoMapper;
using Domain;
using Moq;
using Xunit;

namespace Application.Tests.Activities
{
    public class EditTest : TestBase
    {
        private readonly IMapper _mapper;
        
        public EditTest()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = mockMapper.CreateMapper();
        }

        [Fact]
        public void Should_Edit_Activity()
        {
            var context = GetDbContext();

            var activity = new Activity
            {
                Id = 1,
                Title = "Test Activity",
                Description = "Activity Description",
//                GeoCoordinate = new GeoCoordinate
//                {
//                    Latitude = 20,
//                    Longitude = 40
//                }
            };

            context.Activities.Add(activity);
            context.SaveChanges();

            var activityEditCommand = new Edit.Command
            {
                Activity = new Edit.ActivityData
                {
                    Title = "Updated Title"
                },
                Id = 1
            };
            
            var sut = new Edit.Handler(context, _mapper);
            var result = sut.Handle(activityEditCommand, CancellationToken.None).Result;
            
            Assert.Equal("Updated Title", result.Title);
            Assert.Equal("Activity Description", result.Description);
        }
        
    }
}