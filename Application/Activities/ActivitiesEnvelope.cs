using System.Collections.Generic;
using Domain;

namespace Application.Activities
{
    public class ActivitiesEnvelope
    {
        public List<Activity> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}