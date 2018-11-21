using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        public class ActivityData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
            public GeoCoordinate GeoCoordinate { get; set; }
        }

        public class Command : IRequest<Activity>
        {
            public ActivityData Activity { get; set; }
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Activity>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<Activity> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.Include(x => x.GeoCoordinate).FirstOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = Constants.NOT_FOUND });

                activity.Title = request.Activity.Title ?? activity.Title;
                activity.Description = request.Activity.Description ?? activity.Description;
                activity.Date = request.Activity.Date ?? activity.Date;
                activity.City = request.Activity.City ?? activity.City;
                activity.Venue = request.Activity.Venue ?? activity.Venue;

                if (activity.GeoCoordinate != null)
                {
                    activity.GeoCoordinate.Latitude = activity.GeoCoordinate.Latitude;
                    activity.GeoCoordinate.Longitude = activity.GeoCoordinate.Longitude;
                }
                
                await _context.SaveChangesAsync(cancellationToken);

                return activity;
            }
        }
    }
}