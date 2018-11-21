using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using AutoMapper;
using Domain;
using Infrastructure.Errors;
using Xunit;

namespace Application.Tests.Activities
{
    public class DetailsTest : TestBase
    {
        private readonly IMapper _mapper;

        public DetailsTest()
        {
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfile()); });
            _mapper = mockMapper.CreateMapper();
        }

        [Fact]
        public void Should_get_details()
        {
            var context = GetDbContext();
            context.Activities.Add(new Activity {Id = 1, Title = "Test Activity 1"});
            context.Activities.Add(new Activity {Id = 2, Title = "Test Activity 2"});
            context.SaveChanges();

            var sut = new Details.Handler(context, _mapper);
            var result = sut.Handle(new Details.Query(1), CancellationToken.None).Result;

            Assert.Equal("Test Activity 1", result.Title);
        }

        [Fact]
        public async Task Should_Return_Rest_Exception_If_Null()
        {
            var context = GetDbContext();
            
            var sut = new Details.Handler(context, _mapper);
            
            await Assert.ThrowsAsync<RestException>(() => sut.Handle(new Details.Query(1), CancellationToken.None));
        }
    }
}