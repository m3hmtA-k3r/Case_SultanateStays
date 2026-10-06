namespace SultanateStays.Models
{
    public record HotelDetails(
        int Id,
        string Name,
        string Address,
        string City,
        string District,
        string Zip,
        double Latitude,
        double Longitude,
        int ReviewCount,
        List<string> Facilities,
        List<string> PhotoUrls);
}
