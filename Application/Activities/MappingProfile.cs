using System.Collections.Generic;
using Application.Attendances;
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
            CreateMap<List<ActivityAttendee>, List<Attendee>>();
        }
    }
}