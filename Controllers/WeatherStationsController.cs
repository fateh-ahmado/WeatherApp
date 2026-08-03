using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using Microsoft.AspNetCore.Authorization;

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
                .Include(s => s.Observations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stasjon == null)
            {
                return NotFound();
            }

            return View(stasjon);
        }
        // Create (GET)
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Create (POST) – tar imot og lagrer skjemaet etter den er fylt ut.
        [Authorize]
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
        // U (Update/Edit)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stasjon = await _context.WeatherStations.FindAsync(id);

            if (stasjon == null)
            {
                return NotFound();
            }

            return View(stasjon);
        }

        //Edit (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WeatherStation stasjon)
        {
            if (id != stasjon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.WeatherStations.Update(stasjon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(stasjon);
        }
        //Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var stasjon = await _context.WeatherStations
                .FirstOrDefaultAsync(s => s.Id == id);

            if (stasjon == null)
            {
                return NotFound();
            }

            return View(stasjon);
        }
        //Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stasjon = await _context.WeatherStations.FindAsync(id);

            if (stasjon != null)
            {
                _context.WeatherStations.Remove(stasjon);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}