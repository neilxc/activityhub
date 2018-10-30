using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser<int>
    {
        public List<FollowedPeople> Following { get; set; }
        public List<FollowedPeople> Followers { get; set; }
    }
}