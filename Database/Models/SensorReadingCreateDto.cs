using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class SensorReadingCreateDto
    {
        [Required]
        public string DeviceId { get; set; } = string.Empty;

        [Range(-40, 85)]
        public double Temperature { get; set; }

        [Range(0, 100)]
        public double Humidity { get; set; }

        [Range(300, 1100)]
        public double Pressure { get; set; }
    }
}
