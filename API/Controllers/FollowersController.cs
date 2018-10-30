using System.Threading.Tasks;
using Application.Followers;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/profiles")]
    public class FollowersController : BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<Profile> Follow(string username)
        {
            return await Mediator.Send(new Add.Command(username));
        }

        [HttpDelete("{username}/follow")]
        public async Task<Profile> Unfollow(string username)
        {
            return await Mediator.Send(new Delete.Command(username));
        }
    }
}