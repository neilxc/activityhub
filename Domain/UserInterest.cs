namespace Domain
{
    public class UserInterest
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int InterestId { get; set; }
        public Interest Interest { get; set; }
    }
}