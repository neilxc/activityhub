using Domain;

namespace Application.Profiles
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, Profile>();
        }
    }
}