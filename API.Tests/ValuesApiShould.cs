using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Values;
using API.Controllers;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.Tests
{
    public class ValuesApiShould
    {
        [Fact]
        public async void ReturnValues()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<IRequest>(), CancellationToken.None))
                .Returns(It.IsAny<Task<Unit>>());
            
            var sut = new ValuesController(mediator.Object);

            await sut.Get();

            mediator.Verify(x => x.Send(It.IsAny<List.Query>(), CancellationToken.None));
        }

        [Fact]
        public async void ReturnSingleValue()
        {
            const int valueId = 1;
            var mediator = new Mock<IMediator>();
            var sut = new ValuesController(mediator.Object);

            await sut.Get(valueId);
            
            mediator.Verify(x => x.Send(It.Is<Details.Query>(y => y.Id == valueId), CancellationToken.None), Times.Once);
        }

//        [Fact]
//        public async void ReturnCurrentUserFromContext()
//        {
//            var mediator = new Mock<IMediator>();
//            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, "1"),
//            }));
//
//            var controller = new ValuesController(mediator.Object)
//            {
//                ControllerContext = new ControllerContext 
//                    {HttpContext = new DefaultHttpContext 
//                        {User = user}}
//            };
//        }
    }
}