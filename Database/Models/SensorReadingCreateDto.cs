namespace Database.Models
{
    public class SensorReadingCreateDto
    {
        public string DeviceId { get; set; } = string.Empty;

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Pressure { get; set; }
    }
}
