namespace ApiClient
{
    public class SensorReadingDto
    {
        public int Id { get; set; }

        public string DeviceId { get; set; } = string.Empty;

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Pressure { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
