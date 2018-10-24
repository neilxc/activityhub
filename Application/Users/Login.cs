using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users
{
    public class Login
    {
        public class UserData
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Command : IRequest<User>
        {
            public UserData User { get; set; }
        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;
            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator)
            {
                _jwtGenerator = jwtGenerator;
                _signInManager = signInManager;
                _userManager = userManager;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.User.Email);
                if (user == null)
                    throw new Exception("Failed to login");
                
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.User.Password, false);

                if (result.Succeeded)
                {
                    var userToReturn = new User();
                    userToReturn.Username = user.UserName;
                    userToReturn.Email = user.Email;
                    userToReturn.Token = _jwtGenerator.CreateToken(user);
                    return userToReturn;
                }

                throw new Exception("Unauthorised");
            }
        }
    }
}