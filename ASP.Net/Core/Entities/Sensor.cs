using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Sensor: EntityObject
    {
        public string Location { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public ICollection<Measurement> Readings { get; set; } = new List<Measurement>();

    }
}
