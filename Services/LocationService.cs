using System.Text.Json; //Gir tilgang til å lese og skrive JSON-data
using WeatherApp.Models; // gir tilgang til Coordinates-klassen

namespace WeatherApp.Services
{
    public class LocationService
    {
        // HttpClient brukes til å sende HTTP-forespørsler og motta HTTP-responser over internett.
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherApp"); //User-Agent tekstbit som følger med hver eneste forespørsel, og forteller mottakeren:
        }

        public async Task<Coordinates?> FindCoordinatesAsync(string cityName)
        {
            var url = $"https://ws.geonorge.no/stedsnavn/v1/navn?sok={cityName}&fuzzy=true&utkoordsys=4258&treffPerSide=1&side=1";

            var response = await _httpClient.GetStringAsync(url); // Sender en HTTP GET-forespørsel til Geonorge API for å hente koordinater basert på bynavn
            using var doc = JsonDocument.Parse(response); // Oversetter JSON-responsen til et JsonDocument-objekt for at C# skal kunne lese dataene
            var navn = doc.RootElement.GetProperty("navn"); // Henter "navn" arrayet fra JSON-responsen

            if (navn.GetArrayLength() == 0)
            {
                return null;
            }

            var forsteTreff = navn[0]; // Henter det første treffet fra "navn" arrayet fra JSON-filen.

            var harKommuner = forsteTreff.TryGetProperty("kommuner", out var kommunerVerdi)
                && kommunerVerdi.ValueKind == JsonValueKind.Array
                && kommunerVerdi.GetArrayLength() > 0;

            if (!harKommuner)
            {
                return null;
            }

            var punkt = forsteTreff.GetProperty("representasjonspunkt");

            return new Coordinates
            {
                Latitude = punkt.GetProperty("nord").GetDouble(),
                Longitude = punkt.GetProperty("øst").GetDouble()
            };
        }
    }
}