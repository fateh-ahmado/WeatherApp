using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class WeatherStation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Stedsnavn er påkrevd")]
        [StringLength(100, ErrorMessage = "Stedsnavn kan ikke være lengre enn 100 tegn")]
        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public List<WeatherObservation> Observations { get; set; } = new();
    }
}