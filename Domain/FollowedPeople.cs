namespace Domain
{
    public class FollowedPeople
    {
        public int ObserverId { get; set; }
        public AppUser Observer { get; set; }

        public int TargetId { get; set; }
        public AppUser Target { get; set; }
    }
}