using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<ActivitiesEnvelope> 
        { 
            public Query(string username, string activityFilter, int? limit, int? offset)
            {
                ActivityFilter = activityFilter;
                Limit = limit;
                Offset = offset;
                UserName = username;
            }
            public string ActivityFilter { get; set; }
            public string UserName { get; set; }
            public int? Limit { get; }
            public int? Offset { get; set; }
        }


        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                IQueryable<Activity> queryable = _context.Activities.GetAllData();

                if (!string.IsNullOrEmpty(request.UserName)) 
                {
                    queryable = queryable
                        .Where(x => x.Attendees
                        .Any(y => y.AppUser.UserName == request.UserName));
                }

                switch (request.ActivityFilter) 
                {
                    case "past":
                        queryable = queryable
                            .Where(x => x.Date <= DateTime.Now)
                            .OrderByDescending(x => x.Date);
                        break;
                    case "future":
                        queryable = queryable
                            .Where(x => x.Date >= DateTime.Now)
                            .OrderBy(x => x.Date);
                        break;
                    case "hosting":
                        queryable = queryable
                            .Where(x => x.Attendees
                            .Any(y => y.AppUser.UserName == request.UserName && y.IsHost == true))
                            .OrderByDescending(x => x.Date);
                        break;
                }

                var activitiesFromDb = await queryable
                    .Skip(request.Offset ?? 0)
                    .Take(request.Limit ?? 3)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var activities = _mapper.Map<List<ActivityDto>>(activitiesFromDb);

                return new ActivitiesEnvelope
                {
                    Activities = activities,
                    ActivityCount = activities.Count
                };
            }
        }
    }
}