using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Activities;
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
    public class Delete
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
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<ActivityDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.GetAllData()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = Constants.NOT_FOUND});
                
                var user = await _userManager.FindByNameAsync(_currentUserAccessor.GetCurrentUsername());

                var attendance = await _context.ActivityAttendees.FirstOrDefaultAsync(
                    x => x.ActivityId == activity.Id && x.AppUserId == user.Id, cancellationToken
                );

                if (attendance != null)
                {
                    _context.ActivityAttendees.Remove(attendance);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var activityDto = _mapper.Map<Activity, ActivityDto>(activity);

                return activityDto;
            }
        }
    }
}