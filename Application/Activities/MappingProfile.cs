using AutoMapper;
using Domain;
using static Application.Activities.Create;

namespace Application.Activities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ActivityData, Activity>();
        }
    }
}