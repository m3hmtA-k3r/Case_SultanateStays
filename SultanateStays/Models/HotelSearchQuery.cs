using System.ComponentModel.DataAnnotations;

namespace SultanateStays.Models
{
    public class HotelSearchQuery
    {
        [Required(ErrorMessage = "Lütfen bir şehir girin.")]
        public string Destination { get; set; } = string.Empty;

        public DateOnly CheckIn { get; set; }

        public DateOnly CheckOut { get; set; }

        [Range(1, 10)]
        public int Adults { get; set; } = 2;

        [Range(0, 6)]
        public int Children { get; set; }

        [Range(1, 5)]
        public int Rooms { get; set; } = 1;

        public string Currency { get; set; } = "EUR";

        public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;
    }
}
