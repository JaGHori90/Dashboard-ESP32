using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Measurement :EntityObject
    {
        public int SensorId { get; set; }

        [ForeignKey(nameof(SensorId))]
        public Sensor? Sensor { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double AirPressure { get; set; }

        public DateTime MeasuredAt { get; set; }
    }
}
