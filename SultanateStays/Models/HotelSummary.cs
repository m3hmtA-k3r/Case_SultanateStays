namespace SultanateStays.Models
{
    public record HotelSummary(
        int Id,
        string Name,
        string PhotoUrl,
        double ReviewScore,
        string ReviewScoreWord,
        int ReviewCount,
        decimal TotalPrice,
        string Currency,
        int StarRating);
}
