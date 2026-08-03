using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly LocationService _locationService;
    private readonly WeatherForecastService _weatherForecastService;

    public HomeController(
        ApplicationDbContext context,
        LocationService locationService,
        WeatherForecastService weatherForecastService)
    {
        _context = context;
        _locationService = locationService;
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stasjoner = await _context.WeatherStations
            .Include(s => s.Observations)
            .ToListAsync();
        return View(stasjoner);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string cityName)
    {
        ViewBag.CityName = cityName;

        var coordinates = await _locationService.FindCoordinatesAsync(cityName);

        if (coordinates == null)
        {
            ViewBag.ErrorMessage = "Fant ikke stedet. Prøv et annet navn.";
        }
        else
        {
            var forecast = await _weatherForecastService.GetForecastAsync(coordinates);

            if (forecast == null)
            {
                ViewBag.ErrorMessage = "Klarte ikke hente værvarsel for dette stedet.";
            }
            else
            {
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

                ViewBag.SearchResult = forecast;
            }
        }

        var stasjoner = await _context.WeatherStations
            .Include(s => s.Observations)
            .ToListAsync();
        return View(stasjoner);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}