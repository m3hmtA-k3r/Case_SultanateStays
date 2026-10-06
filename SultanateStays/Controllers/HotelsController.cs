using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SultanateStays.Models;
using SultanateStays.Models.RapidApi;
using SultanateStays.Options;
using SultanateStays.ViewModels;

namespace SultanateStays.Controllers
{
    public class HotelsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiOptions _options;
        private readonly IWebHostEnvironment _environment;

        public HotelsController(IHttpClientFactory httpClientFactory, IOptions<RapidApiOptions> options, IWebHostEnvironment environment)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _environment = environment;
        }

        public async Task<IActionResult> Search(HotelSearchQuery query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.Destination))
            {
                return RedirectToAction("Index", "Home");
            }

            var dateError = ValidateDates(query);
            if (dateError is not null)
            {
                return View(new HotelSearchViewModel { Query = query, ErrorMessage = dateError });
            }

            try
            {
                var destinations = await GetAsync<DestinationSearchResponse.Rootobject>(
                    "api/v1/hotels/searchDestination",
                    "destination.json",
                    new Dictionary<string, string?> { ["query"] = query.Destination },
                    ct);

                var city = destinations.data?.FirstOrDefault(d =>
                    d.search_type == "city" &&
                    string.Equals(d.name, query.Destination.Trim(), StringComparison.OrdinalIgnoreCase));

                if (city is null)
                {
                    return View(new HotelSearchViewModel
                    {
                        Query = query,
                        ErrorMessage = $"\"{query.Destination}\" için şehir bulunamadı. Lütfen farklı bir şehir deneyin."
                    });
                }

                var hotels = await GetAsync<SearchHotelsResponse>(
                    "api/v1/hotels/searchHotels",
                    "hotels.json",
                    new Dictionary<string, string?>
                    {
                        ["dest_id"] = city.dest_id,
                        ["search_type"] = "CITY",
                        ["arrival_date"] = query.CheckIn.ToString("yyyy-MM-dd"),
                        ["departure_date"] = query.CheckOut.ToString("yyyy-MM-dd"),
                        ["adults"] = query.Adults.ToString(),
                        ["room_qty"] = query.Rooms.ToString(),
                        ["currency_code"] = query.Currency,
                        ["page_number"] = "1",
                    },
                    ct);

                var summaries = hotels.data?.hotels?.Select(ToSummary).ToList() ?? new List<HotelSummary>();

                return View(new HotelSearchViewModel { Query = query, Hotels = summaries });
            }
            catch (HttpRequestException)
            {
                return View(new HotelSearchViewModel
                {
                    Query = query,
                    ErrorMessage = "Oteller şu anda getirilemedi. Lütfen daha sonra tekrar deneyin."
                });
            }
        }

        public async Task<IActionResult> Details(int id, HotelSearchQuery query, CancellationToken ct)
        {
            try
            {
                var response = await GetAsync<HotelDetailsResponse>(
                    "api/v1/hotels/getHotelDetails",
                    "details.json",
                    new Dictionary<string, string?>
                    {
                        ["hotel_id"] = id.ToString(),
                        ["currency_code"] = query.Currency,
                        ["adults"] = query.Adults.ToString(),
                        ["room_qty"] = query.Rooms.ToString(),
                        ["arrival_date"] = query.CheckIn.ToString("yyyy-MM-dd"),
                        ["departure_date"] = query.CheckOut.ToString("yyyy-MM-dd"),
                    },
                    ct);

                if (response.data is null)
                {
                    return NotFound();
                }

                return View(new HotelDetailsViewModel { Hotel = ToDetails(response.data), Query = query });
            }
            catch (HttpRequestException)
            {
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        private async Task<T> GetAsync<T>(string path, string mockFile, IDictionary<string, string?> query, CancellationToken ct)
        {
            if (_options.UseMock)
            {
                await using var stream = System.IO.File.OpenRead(Path.Combine(_environment.ContentRootPath, "mock", mockFile));
                return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: ct)
                    ?? throw new InvalidOperationException($"{mockFile} okunamadı.");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.BaseUrl);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _options.ApiKey);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", _options.Host);

            using var response = await client.GetAsync(QueryHelpers.AddQueryString(path, query), ct);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<T>(body)
                ?? throw new InvalidOperationException("API yanıtı okunamadı.");
        }

        private static HotelSummary ToSummary(SearchHotelsResponse.Hotel hotel)
        {
            var property = hotel.property;
            var price = property.priceBreakdown.grossPrice;

            return new HotelSummary(
                hotel.hotel_id,
                property.name,
                property.photoUrls?.FirstOrDefault() ?? string.Empty,
                property.reviewScore,
                property.reviewScoreWord,
                property.reviewCount,
                Math.Round((decimal)price.value, 0),
                price.currency,
                property.propertyClass);
        }

        private static HotelDetails ToDetails(HotelDetailsResponse.Data data)
        {
            var facilities = data.facilities_block?.facilities?.Select(f => f.name).ToList() ?? new List<string>();

            return new HotelDetails(
                data.hotel_id,
                data.hotel_name,
                data.address,
                data.city,
                data.district,
                data.zip,
                data.latitude,
                data.longitude,
                data.review_nr,
                facilities,
                new List<string>());
        }

        private static string? ValidateDates(HotelSearchQuery query)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (query.CheckIn < today)
            {
                return "Giriş tarihi bugünden önce olamaz.";
            }

            if (query.CheckOut <= query.CheckIn)
            {
                return "Çıkış tarihi giriş tarihinden sonra olmalı.";
            }

            return null;
        }
    }
}
