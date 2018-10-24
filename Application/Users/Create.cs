using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using FluentValidation;
using Infrastructure.Errors;
using Infrastructure.Security;
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

        public class UserDataValidator : AbstractValidator<UserData>
        {
            public UserDataValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
                RuleFor(x => x.Email).NotNull().EmailAddress().NotEmpty();
                RuleFor(x => x.Password).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<User>
        {
            public UserData User { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
            }
        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IMapper _mapper;

            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IMapper mapper)
            {
                _mapper = mapper;
                _userManager = userManager;
                _jwtGenerator = jwtGenerator;
                _context = context;
            }
            public async Task<User> Handle(Command request, CancellationToken cancellationToken)
            {
                if (await _context.Users.Where(x => x.Email == request.User.Email).AnyAsync(cancellationToken))
                    throw new RestException(HttpStatusCode.BadRequest, new {Email = Constants.IN_USE});

                if (await _context.Users.Where(x => x.UserName == request.User.Username).AnyAsync(cancellationToken))
                    throw new RestException(HttpStatusCode.BadRequest, new {UserName = Constants.IN_USE});

                var user = new AppUser
                {
                    Email = request.User.Email,
                    UserName = request.User.Username
                };

                var result = await _userManager.CreateAsync(user, request.User.Password);

                if (result.Succeeded)
                {
                    var userFromDb = await _userManager.FindByNameAsync(request.User.Username);
                    var userToReturn = _mapper.Map<AppUser, User>(userFromDb);
                    userToReturn.Token = _jwtGenerator.CreateToken(userFromDb);
                    return userToReturn;
                }

                throw new Exception("Something went wrong");
            }
        }
    }
}