using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    public class WeatherSearchController : Controller
    {
        private readonly LocationService _locationService;
        private readonly WeatherForecastService _weatherForecastService;
        private readonly ApplicationDbContext _context;

        public WeatherSearchController(
            LocationService locationService,
            WeatherForecastService weatherForecastService,
            ApplicationDbContext context)
        {
            _locationService = locationService;
            _weatherForecastService = weatherForecastService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string cityName)
        {
            ViewBag.CityName = cityName;

            var coordinates = await _locationService.FindCoordinatesAsync(cityName);

            if (coordinates == null)
            {
                ViewBag.ErrorMessage = "Fant ikke stedet. Prøv et annet navn.";
                return View();
            }

            var forecast = await _weatherForecastService.GetForecastAsync(coordinates);

            if (forecast == null)
            {
                ViewBag.ErrorMessage = "Klarte ikke hente værvarsel for dette stedet.";
                return View();
            }

            var stasjon = await _context.WeatherStations
                .FirstOrDefaultAsync(s => s.Name == cityName);

            if (stasjon == null)
            {
                stasjon = new WeatherStation
                {
                    Name = cityName,
                    Latitude = coordinates.Latitude,
                    Longitude = coordinates.Longitude
                };
                _context.WeatherStations.Add(stasjon);
                await _context.SaveChangesAsync();
            }

            forecast.WeatherStationId = stasjon.Id;
            _context.WeatherObservations.Add(forecast);
            await _context.SaveChangesAsync();

            return View((object?)forecast);
        }
    }
}