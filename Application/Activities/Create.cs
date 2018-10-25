using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class ActivityData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
            public GeoCoordinate GeoCoordinate { get; set; }
        }

        public class ActivityDataValidator : AbstractValidator<ActivityData>
        {
            public ActivityDataValidator()
            {
                RuleFor(x => x.Title).NotEmpty().NotNull();
                RuleFor(x => x.Description).NotEmpty().NotNull();
                RuleFor(x => x.Date).NotEmpty().NotNull();
                RuleFor(x => x.City).NotEmpty().NotNull();
                RuleFor(x => x.Venue).NotEmpty().NotNull();
                RuleFor(x => x.GeoCoordinate.Latitude).NotEmpty().NotNull();
                RuleFor(x => x.GeoCoordinate.Longitude).NotNull().NotEmpty();
            }
        }

        public class Command : IRequest<Activity>
        {
            public ActivityData Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).NotNull().SetValidator(new ActivityDataValidator());
            }
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
                var activity = _mapper.Map<ActivityData, Activity>(request.Activity);

                await _context.Activities.AddAsync(activity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return activity;
            }
        }
    }
}