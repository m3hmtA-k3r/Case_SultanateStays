#nullable disable

using System.Text.Json;

namespace SultanateStays.Models.RapidApi
{
    public class HotelPhotosResponse
    {
        public bool status { get; set; }
        public JsonElement message { get; set; }
        public long timestamp { get; set; }
        public List<Photo> data { get; set; }

        public class Photo
        {
            public long id { get; set; }
            public string url { get; set; }
        }
    }
}
