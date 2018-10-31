using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Infrastructure.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int activityId, int commentId)
            {
                ActivityId = activityId;
                CommentId = commentId;
            }

            public int ActivityId { get; }
            public int CommentId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ActivityId).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);
                
                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Activity = Constants.NOT_FOUND});
                
                var comment = activity.Comments.FirstOrDefault(x => x.Id == request.CommentId);

                if (comment == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Comment = Constants.NOT_FOUND});

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}