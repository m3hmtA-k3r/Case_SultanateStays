using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SultanateStays.Models;

namespace SultanateStays.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = new HotelSearchQuery
            {
                Destination = "İstanbul",
                CheckIn = today.AddDays(7),
                CheckOut = today.AddDays(10),
            };

            return View(query);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
