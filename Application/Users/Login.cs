using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Users
{
    public class Login
    {
        public class UserData
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

//        public class UserDataValidator : AbstractValidator<UserData>
//        {
//            public UserDataValidator()
//            {
//                RuleFor(x => x.Email).NotNull().EmailAddress().NotEmpty();
//                RuleFor(x => x.Password).NotNull().NotEmpty();
//            }
//        }

        public class Command : IRequest<User>
        {
            public UserData User { get; set; }
        }

//        public class CommandValidator : AbstractValidator<Command>
//        {
//            public CommandValidator()
//            {
//                RuleFor(x => x.User).NotNull().SetValidator(new UserDataValidator());
//            }
//        }

        public class Handler : IRequestHandler<Command, User>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly IMapper _mapper;
            public Handler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJwtGenerator jwtGenerator, IMapper mapper)
            {
                _mapper = mapper;
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
                    var userToReturn = _mapper.Map<AppUser, User>(user);
                    userToReturn.Token = _jwtGenerator.CreateToken(user);
                    return userToReturn;
                }

                throw new Exception("Unauthorised");
            }
        }
    }
}