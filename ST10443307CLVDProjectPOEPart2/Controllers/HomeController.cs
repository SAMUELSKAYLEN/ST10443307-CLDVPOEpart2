using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ST10443307CLVDProjectPOEPart2.Models;

namespace ST10443307CLVDProjectPOEPart2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact() 
        {
            return View();
        }

        public IActionResult Venues()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Bookings()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
