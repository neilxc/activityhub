using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class Create
    {
        public class UserData
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Command : IRequest<User>
        {
            public UserData User { get; set; }
        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            public Handler(DataContext context, UserManager<AppUser> userManager)
            {
                _userManager = userManager;
                _context = context;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email == request.User.Email).AnyAsync(cancellationToken))
                    throw new Exception("Email already in use");

                if (await _context.Users.Where(x => x.UserName == request.User.Username).AnyAsync(cancellationToken))
                    throw new Exception("Username already in use");
                
                var user = new AppUser
                {
                    Email = request.User.Email,
                    UserName = request.User.Username
                };

                var result = await _userManager.CreateAsync(user, request.User.Password);

                if (result.Succeeded)
                {
                    var userFromDb = await _userManager.FindByNameAsync(request.User.Username);
                    var userToReturn = new User();
                    userToReturn.Username = userFromDb.UserName;
                    userToReturn.Email = userFromDb.Email;
                    return userToReturn;
                }

                throw new Exception("Something went wrong");
            }
        }
    }
}