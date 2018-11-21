using AutoMapper;
using Domain;

namespace Application.Photos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Photo, PhotoDto>();
        }
    }
}