#nullable disable

using System.Text.Json;

namespace SultanateStays.Models.RapidApi
{
    public class HotelDetailsResponse
    {
        public bool status { get; set; }
        public JsonElement message { get; set; }
        public long timestamp { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public int ufi { get; set; }
            public int hotel_id { get; set; }
            public string hotel_name { get; set; }
            public string url { get; set; }
            public int review_nr { get; set; }
            public string arrival_date { get; set; }
            public string departure_date { get; set; }
            public string currency_code { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string address { get; set; }
            public string zip { get; set; }
            public string city { get; set; }
            public string district { get; set; }
            public string countrycode { get; set; }
            public FacilitiesBlock facilities_block { get; set; }
        }

        public class FacilitiesBlock
        {
            public string type { get; set; }
            public string name { get; set; }
            public List<Facility> facilities { get; set; }
        }

        public class Facility
        {
            public string name { get; set; }
            public string icon { get; set; }
        }
    }
}
