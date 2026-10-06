using SultanateStays.Models;

namespace SultanateStays.ViewModels
{
    public class HotelSearchViewModel
    {
        public required HotelSearchQuery Query { get; init; }

        public List<HotelSummary>? Hotels { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
