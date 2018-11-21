using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly ICloudinaryAccount _cloudinary;

            public Handler(DataContext context, ICurrentUserAccessor currentUserAccessor, ICloudinaryAccount cloudinary)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _cloudinary = cloudinary;
            }
            
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var username = _currentUserAccessor.GetCurrentUsername();

                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
                
                if (user.Photos.All(p => p.Id != request.Id))
                {
                    throw new RestException(HttpStatusCode.Unauthorized);
                }
                
                var photoFromDb = await _context.Photos.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
                
                if (photoFromDb.IsMain)
                    throw new RestException(HttpStatusCode.BadRequest);

                _cloudinary.DeletePhotoForUser(photoFromDb);
                
                return Unit.Value;
            }
        }
    }
}