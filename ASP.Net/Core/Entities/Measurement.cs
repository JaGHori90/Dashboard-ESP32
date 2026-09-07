using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Range(-30,100)]
        public double Temperature { get; set; }

        [Range(0,100)]
        public double Humidity { get; set; } 

        [Range(800,1100)]
        public double AirPressure { get; set; }

        public DateTime MeasuredAt { get; set; }
    }
}
