using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Create
    {
        public class PhotoData
        {
            public IFormFile File { get; set; }
        }
        
//        public class PhotoDataValidator : AbstractValidator<PhotoData>
//        {
//            public PhotoDataValidator()
//            {
//                RuleFor(x => x.File.Name).NotNull();
//            }
//        }
        
        public class Command : IRequest<PhotoDto>
        {
            public PhotoData Photo { get; set; }
        }
        
//        public class CommandValidator : AbstractValidator<Command>
//        {
//            public CommandValidator()
//            {
//                RuleFor(x => x.Photo).NotNull().SetValidator(new PhotoDataValidator());
//            }
//        }

        public class Handler : IRequestHandler<Command, PhotoDto>
        {
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly DataContext _context;
            private readonly ICloudinaryAccount _cloudinary;
            private readonly IMapper _mapper;

            public Handler(ICurrentUserAccessor currentUserAccessor, 
                DataContext context, 
                ICloudinaryAccount cloudinary,
                IMapper mapper)
            {
                _currentUserAccessor = currentUserAccessor;    
                _context = context;
                _cloudinary = cloudinary;
                _mapper = mapper;
            }
            
            public async Task<PhotoDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var username = _currentUserAccessor.GetCurrentUsername();

                var user = await _context.Users.Include(p => p.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
                
                var photo = _cloudinary.AddPhotoForUser(request.Photo.File);

                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;
                
                user.Photos.Add(photo);

                await _context.SaveChangesAsync(cancellationToken);

                var photoDto = _mapper.Map<Photo, PhotoDto>(photo);

                return photoDto;
            }
        }
    }
}