using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        public class Query : IRequest<List<CommentDto>>
        {
            public Query(int activityId)
            {
                ActivityId = activityId;
            }

            public int ActivityId { get; }
        }

        public class Handler : IRequestHandler<Query, List<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<CommentDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(x => x.Comments).ThenInclude(x => x.Author)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = Constants.NOT_FOUND});

                var comments = _mapper.Map<List<Comment>, List<CommentDto>>(activity.Comments);

                return comments;
            }
        }
    }
}