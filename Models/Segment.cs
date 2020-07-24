using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Segment
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public double timespan_seconds { get; set; }
    }
}
