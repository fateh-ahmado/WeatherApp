using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    // public class ApplicationDbContext : DbContext 
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> // dette brukes nå for å håndtere brukerautentisering og autorisasjon i applikasjonen.
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherStation> WeatherStations { get; set; }
        public DbSet<WeatherObservation> WeatherObservations { get; set; }
    }
}