using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherForecastService
    {
        private readonly HttpClient _httpClient;

        public WeatherForecastService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherApp/1.0");
        }

        public async Task<WeatherObservation?> GetForecastAsync(Coordinates coordinates)
        {
            var url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={coordinates.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            var response = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);

            var forsteTidspunkt = doc.RootElement
                .GetProperty("properties")
                .GetProperty("timeseries")[0];

            var tidspunkt = forsteTidspunkt.GetProperty("time").GetDateTime();
            var detaljer = forsteTidspunkt.GetProperty("data").GetProperty("instant").GetProperty("details");

            var temperatur = detaljer.GetProperty("air_temperature").GetDouble();
            var vindkast = detaljer.TryGetProperty("wind_speed", out var vindVerdi)
                ? vindVerdi.GetDouble()
                : 0;

            return new WeatherObservation
            {
                Timestamp = tidspunkt,
                Temperature = temperatur,
                WindGust = vindkast
            };
        }
    }
}