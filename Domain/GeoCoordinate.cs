using Newtonsoft.Json;

namespace Domain
{
    public class GeoCoordinate
    {
        [JsonIgnore]
        public int Id { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}