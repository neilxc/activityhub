using System;

namespace Domain
{
    public class ActivityAttendee
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }
        public DateTime DateJoined { get; set; }
        public bool IsHost { get; set; }
    }
}