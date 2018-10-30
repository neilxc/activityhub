using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ActivityDto>
        {
            public Query(int id)
            {
                Id = id;
            }
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .GetAllData()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = Constants.NOT_FOUND});
                
                return _mapper.Map<ActivityDto>(activity);;
            }
        }
    }
}