using System;

namespace Application.Attendances
{
    public class Attendee
    {
        public string UserName { get; set; }
        public DateTime DateJoined { get; set; }
        public string Image { get; set; }
        public bool IsHost { get; set; }
    }
}