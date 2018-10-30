using Newtonsoft.Json;

namespace Application.Profiles
{
    public class Profile
    {
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        [JsonProperty("following")]
        public bool IsFollowed { get; set; }
    }
}