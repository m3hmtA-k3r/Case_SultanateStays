#nullable disable

using System.Text.Json;

namespace SultanateStays.Models.RapidApi
{
    public class SearchHotelsResponse
    {
        public bool status { get; set; }
        public JsonElement message { get; set; }
        public long timestamp { get; set; }
        public Data data { get; set; }

        public class Data
        {
            public List<Hotel> hotels { get; set; }
            public List<Meta> meta { get; set; }
        }

        public class Meta
        {
            public string title { get; set; }
        }

        public class Hotel
        {
            public int hotel_id { get; set; }
            public string accessibilityLabel { get; set; }
            public Property property { get; set; }
        }

        public class Property
        {
            public int id { get; set; }
            public string name { get; set; }
            public int ufi { get; set; }
            public int position { get; set; }
            public int rankingPosition { get; set; }
            public int qualityClass { get; set; }
            public int propertyClass { get; set; }
            public int accuratePropertyClass { get; set; }
            public bool isPreferred { get; set; }
            public bool isFirstPage { get; set; }
            public int reviewCount { get; set; }
            public double reviewScore { get; set; }
            public string reviewScoreWord { get; set; }
            public string countryCode { get; set; }
            public string currency { get; set; }
            public string checkinDate { get; set; }
            public string checkoutDate { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public int mainPhotoId { get; set; }
            public int optOutFromGalleryChanges { get; set; }
            public string recommendedUnitsConfigurationLabel { get; set; }
            public string wishlistName { get; set; }
            public Checkin checkin { get; set; }
            public Checkout checkout { get; set; }
            public Pricebreakdown priceBreakdown { get; set; }
            public List<string> photoUrls { get; set; }
        }

        public class Checkin
        {
            public string fromTime { get; set; }
            public string untilTime { get; set; }
        }

        public class Checkout
        {
            public string fromTime { get; set; }
            public string untilTime { get; set; }
        }

        public class Pricebreakdown
        {
            public Grossprice grossPrice { get; set; }
            public Excludedprice excludedPrice { get; set; }
            public Strikethroughprice strikethroughPrice { get; set; }
        }

        public class Grossprice
        {
            public double value { get; set; }
            public string currency { get; set; }
            public string amountRounded { get; set; }
        }

        public class Excludedprice
        {
            public double value { get; set; }
            public string currency { get; set; }
        }

        public class Strikethroughprice
        {
            public double value { get; set; }
            public string currency { get; set; }
            public string amountRounded { get; set; }
        }
    }
}
