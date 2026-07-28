using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    public class WeatherStationsController : Controller
    {
        private readonly ApplicationDbContext _context; // Referanse til databasen som vi kan bruke for å hente og lagre data.

        public WeatherStationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dette viser en liste over alle værstasjoner som er lagret i databasen.
        public async Task<IActionResult> Index()
        {
            var stasjoner = await _context.WeatherStations.ToListAsync(); 
            return View(stasjoner);
        }
        public async Task<IActionResult> Details(int id)
        {
            var stasjon = await _context.WeatherStations
                .FirstOrDefaultAsync(s => s.Id == id); // FirstOrDefaultAsync henter den første stasjonen som matcher id-en, eller null hvis ingen finnes.

            if (stasjon == null)
            {
                return NotFound(); // Dersom stasjonen ikke finnes, returner en 404-feil.
            }

            return View(stasjon);
        }
        // Create (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Create (POST) – tar imot og lagrer skjemaet etter den er fylt ut.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeatherStation stasjon)
        {
            if (ModelState.IsValid)
            {
                _context.WeatherStations.Add(stasjon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(stasjon);
        }
    }
}