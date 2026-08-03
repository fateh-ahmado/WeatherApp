using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Controllers;
using WeatherApp.Data;
using WeatherApp.Models;
using Xunit;

namespace WeatherApp.Tests
{
    public class WeatherStationsControllerTests
    {
        /*
        oppretter en midlertidig, "falsk" database (kun i minnet, ikke en ekte fil), 
        som gir deg mulighet til å teste metodene dine trygt, uten å påvirke den ekte weatherapp.db-*/
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact] //Lapp som settes på metoden for å si at dette er en testmetode
        public async Task Index_ReturnerAlleStasjoner()
        {
            var context = GetInMemoryContext(); // bygger en tom midlertidig database (kun for denne ene testen, forsvinner etterpå)
            context.WeatherStations.Add(new WeatherStation { Name = "Oslo", Latitude = 59.91, Longitude = 10.75 });
            context.WeatherStations.Add(new WeatherStation { Name = "Bergen", Latitude = 60.39, Longitude = 5.32 });
            await context.SaveChangesAsync(); // Legger til objektene i databasen (lagrer dem)

            var controller = new WeatherStationsController(context);

            var result = await controller.Index();
            // Assert: 
//                    Dette brukes til å sjekke at det jeg påstår faktisk stemmer (Selve testen)
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<WeatherStation>>(viewResult.Model);
            Assert.Equal(2, model.Count); // Sjekker at antallet elementer i model nøyaktig 2
        }

        [Fact]
        public async Task Details_ReturnererRiktigStasjon()
        {
            var context = GetInMemoryContext();
            var stasjon = new WeatherStation { Name = "Trondheim", Latitude = 63.43, Longitude = 10.39 };
            context.WeatherStations.Add(stasjon);
            await context.SaveChangesAsync();

            var controller = new WeatherStationsController(context);

            var result = await controller.Details(stasjon.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<WeatherStation>(viewResult.Model);
            Assert.Equal("Trondheim", model.Name);
        }

        [Fact]
        public async Task Details_ReturnererNotFoundForUgyldigId()
        {
            var context = GetInMemoryContext();
            var controller = new WeatherStationsController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_LagrerNyStasjon()
        {
            var context = GetInMemoryContext();
            var controller = new WeatherStationsController(context);
            var nyStasjon = new WeatherStation { Name = "Stavanger", Latitude = 58.97, Longitude = 5.73 };

            var result = await controller.Create(nyStasjon);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var lagretStasjon = await context.WeatherStations.FirstOrDefaultAsync(s => s.Name == "Stavanger");
            Assert.NotNull(lagretStasjon);
        }

        [Fact]
        public async Task DeleteConfirmed_SletterStasjon()
        {
            var context = GetInMemoryContext();
            var stasjon = new WeatherStation { Name = "Kristiansand", Latitude = 58.16, Longitude = 8.00 };
            context.WeatherStations.Add(stasjon);
            await context.SaveChangesAsync();

            var controller = new WeatherStationsController(context);

            var result = await controller.DeleteConfirmed(stasjon.Id);

            Assert.IsType<RedirectToActionResult>(result);

            var slettetStasjon = await context.WeatherStations.FirstOrDefaultAsync(s => s.Id == stasjon.Id);
            Assert.Null(slettetStasjon);
        }
    }
}