using System.Collections.Generic;

namespace Domain
{
    public class Interest
    {
        public int Id { get; set; }
        public List<UserInterest> UserInterests { get; set; }
    }
}