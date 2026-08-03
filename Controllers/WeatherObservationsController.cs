using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace WeatherApp.Controllers
{
    public class WeatherObservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeatherObservationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var malinger = await _context.WeatherObservations
                .Include(o => o.WeatherStation)
                .ToListAsync();

            return View(malinger);
        }
        // Create 
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.WeatherStations = new SelectList(_context.WeatherStations, "Id", "Name");
            return View();
        }
        //Create (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WeatherObservation maling)
        {
            if (ModelState.IsValid)
            {
                _context.WeatherObservations.Add(maling);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.WeatherStations = new SelectList(_context.WeatherStations, "Id", "Name");
            return View(maling);
        }
        //Edit (GET)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var maling = await _context.WeatherObservations.FindAsync(id);

            if (maling == null)
            {
                return NotFound();
            }

            ViewBag.WeatherStations = new SelectList(_context.WeatherStations, "Id", "Name", maling.WeatherStationId);
            return View(maling);
        }
        //Edit (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WeatherObservation maling)
        {
            if (id != maling.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.WeatherObservations.Update(maling);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.WeatherStations = new SelectList(_context.WeatherStations, "Id", "Name", maling.WeatherStationId);
            return View(maling);
        }
        //Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var maling = await _context.WeatherObservations
                .Include(o => o.WeatherStation)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (maling == null)
            {
                return NotFound();
            }

            return View(maling);
        }
        //Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maling = await _context.WeatherObservations.FindAsync(id);

            if (maling != null)
            {
                _context.WeatherObservations.Remove(maling);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}