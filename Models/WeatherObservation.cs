using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherApp.Models
{
    public class WeatherObservation
    {   
        public int Id { get; set; }

        [Required(ErrorMessage = "Tidspunkt er påkrevd")]
        public DateTime Timestamp { get; set; }

        [Required(ErrorMessage = "Temperatur er påkrevd")]
        public double Temperature { get; set; }

        public double WindGust { get; set; }

        [ForeignKey("WeatherStation")]
        public int WeatherStationId { get; set; }

        public WeatherStation? WeatherStation { get; set; }
    }
}