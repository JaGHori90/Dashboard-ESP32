using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Dtos
{
    public class MeasurmentDto
    {
        public int Id { get; set; }
        public int SensorId { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double AirPressure { get; set; }

        public DateTime MeasuredAt { get; set; }

    }
}
