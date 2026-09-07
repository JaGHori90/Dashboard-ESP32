using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient.Dtos
{
    public class CityWeatherDto
    {
        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double MaxTemp { get; set; }

        public double MinTemp { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
