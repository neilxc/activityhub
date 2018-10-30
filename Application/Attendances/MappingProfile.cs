using AutoMapper;
using Domain;

namespace Application.Attendances
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ActivityAttendee, Attendee>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.AppUser.UserName));
        }
    }
}