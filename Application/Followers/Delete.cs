using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Profiles;
using FluentValidation;
using Infrastructure;
using Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class Delete
    {
        public class Command : IRequest<Profile>
        {
            public Command(string username)
            {
                UserName = username;
            }

            public string UserName {get;}
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.UserName)).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Profile>
        {
            private readonly DataContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IProfileReader _profileReader;

            public Handler(DataContext context, ICurrentUserAccessor currentUserAccessor, IProfileReader profileReader)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _profileReader = profileReader;
            }
            public async Task<Profile> Handle(Command request, CancellationToken cancellationToken)
            {
                var target = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
                
                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new {User = Constants.NOT_FOUND});

                var observer = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _currentUserAccessor.GetCurrentUsername());

                var followedPeople = await _context.FollowedPeople
                    .FirstOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id, cancellationToken);

                if (followedPeople != null)
                {
                    _context.FollowedPeople.Remove(followedPeople);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(request.UserName);
            }
        }
    }
}