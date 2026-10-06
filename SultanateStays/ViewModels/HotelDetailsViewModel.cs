using SultanateStays.Models;

namespace SultanateStays.ViewModels
{
    public class HotelDetailsViewModel
    {
        public required HotelDetails Hotel { get; init; }

        public required HotelSearchQuery Query { get; init; }
    }
}
