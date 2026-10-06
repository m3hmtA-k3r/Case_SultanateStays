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

        public HotelsController(IHttpClientFactory httpClientFactory, IOptions<RapidApiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<IActionResult> Search(HotelSearchQuery query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.Destination))
            {
                return RedirectToAction("Index", "Home");
            }

            var validationError = ValidateDates(query) ?? ValidateChildren(query);
            if (validationError is not null)
            {
                return View(new HotelSearchViewModel { Query = query, ErrorMessage = validationError });
            }

            try
            {
                var destinations = await GetAsync<DestinationSearchResponse.Rootobject>(
                    "api/v1/hotels/searchDestination",
                    new Dictionary<string, string?> { ["query"] = query.Destination.Trim() },
                    ct);

                var city = destinations.data?.FirstOrDefault(d => d.search_type == "city");

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
                    WithChildren(
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
                        query),
                    ct);

                var summaries = hotels.data?.hotels?.Select(ToSummary).ToList() ?? new List<HotelSummary>();

                return View(new HotelSearchViewModel { Query = query, Hotels = summaries });
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
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
            if (ValidateChildren(query) is not null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var details = await GetAsync<HotelDetailsResponse>(
                    "api/v1/hotels/getHotelDetails",
                    WithChildren(
                        new Dictionary<string, string?>
                        {
                            ["hotel_id"] = id.ToString(),
                            ["currency_code"] = query.Currency,
                            ["adults"] = query.Adults.ToString(),
                            ["room_qty"] = query.Rooms.ToString(),
                            ["arrival_date"] = query.CheckIn.ToString("yyyy-MM-dd"),
                            ["departure_date"] = query.CheckOut.ToString("yyyy-MM-dd"),
                        },
                        query),
                    ct);

                if (details.data is null)
                {
                    return NotFound();
                }

                var description = await GetAsync<HotelDescriptionResponse>(
                    "api/v1/hotels/getDescriptionAndInfo",
                    new Dictionary<string, string?>
                    {
                        ["hotel_id"] = id.ToString(),
                        ["languagecode"] = "en-us",
                    },
                    ct);

                var photos = await GetAsync<HotelPhotosResponse>(
                    "api/v1/hotels/getHotelPhotos",
                    new Dictionary<string, string?> { ["hotel_id"] = id.ToString() },
                    ct);

                var hotel = ToDetails(
                    details.data,
                    ToDescription(description),
                    photos.data?.Select(p => p.url).ToList() ?? new List<string>());

                return View(new HotelDetailsViewModel { Hotel = hotel, Query = query });
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException)
            {
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        private async Task<T> GetAsync<T>(string path, IDictionary<string, string?> query, CancellationToken ct)
        {
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

        private static Dictionary<string, string?> WithChildren(Dictionary<string, string?> parameters, HotelSearchQuery query)
        {
            if (query.Children > 0)
            {
                parameters["children_age"] = string.Join(",", query.ChildAgeList);
            }

            return parameters;
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

        private static string ToDescription(HotelDescriptionResponse response)
        {
            return response.data?.FirstOrDefault(d => d.descriptiontype_id == 6)?.description ?? string.Empty;
        }

        private static HotelDetails ToDetails(HotelDetailsResponse.Data data, string description, List<string> photoUrls)
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
                description,
                facilities,
                photoUrls);
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

        private static string? ValidateChildren(HotelSearchQuery query)
        {
            if (query.Children == 0)
            {
                return null;
            }

            var ages = query.ChildAgeList;
            if (ages.Count != query.Children || ages.Any(a => a < 0 || a > 17))
            {
                return "Lütfen her çocuk için yaşını seçin (0-17).";
            }

            return null;
        }
    }
}
