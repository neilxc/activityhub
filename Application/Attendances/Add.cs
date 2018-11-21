using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Attendances
{
    public class Add
    {
        public class Command : IRequest<ActivityDto>
        {
            public Command(int id)
            {
                Id = id;
            }
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public Handler(DataContext context, ICurrentUserAccessor currentUserAccessor, UserManager<AppUser> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
                _currentUserAccessor = currentUserAccessor;
                _context = context;
            }
            public async Task<ActivityDto> Handle(Command request, CancellationToken cancellationToken)
            {
                // get the current event from db
                var activity = await _context.Activities
                    .Include(x => x.Attendees)
                    .ThenInclude(a => a.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                var username = _currentUserAccessor.GetCurrentUsername();
                // get the currently logged in user
                var user = await _userManager.FindByNameAsync(_currentUserAccessor.GetCurrentUsername());

                // create the attendance - check to see if it exists first
                var attendance = await _context.ActivityAttendees.FirstOrDefaultAsync(
                        x => x.ActivityId == activity.Id && x.AppUserId == user.Id, cancellationToken);

                if (attendance == null)
                {
                    attendance = new ActivityAttendee()
                    {
                        Activity = activity,
                        ActivityId = activity.Id,
                        AppUser = user,
                        AppUserId = user.Id,
                        DateJoined = DateTime.Now,
                        IsHost = false
                    };

                    await _context.ActivityAttendees.AddAsync(attendance, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var activityDto = _mapper.Map<Activity, ActivityDto>(activity);

                return activityDto;

            }
        }

    }
}