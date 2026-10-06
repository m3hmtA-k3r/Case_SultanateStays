#nullable disable

using System.Text.Json;

namespace SultanateStays.Models.RapidApi
{
    public class HotelDescriptionResponse
    {
        public bool status { get; set; }
        public JsonElement message { get; set; }
        public long timestamp { get; set; }
        public List<Description> data { get; set; }

        public class Description
        {
            public int descriptiontype_id { get; set; }
            public string hotel_id { get; set; }
            public string languagecode { get; set; }
            public string description { get; set; }
        }
    }
}
