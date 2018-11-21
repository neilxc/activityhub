using System;
using Domain;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class UsersFeatureShould
    {
        [Fact]
        public void ReturnUserWhenHandleCreateIsSuccessful()
        {
            var mockStore = Mock.Of<IUserStore<AppUser>>();
            var mockUserManager =
                new Mock<UserManager<AppUser>>(mockStore, null, null, null, null, null, null, null, null);

            mockUserManager
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
        }
    }
}