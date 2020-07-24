using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string flight_id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Company_name { get; set; }
        public string date_time { get; set; }
        public bool isExternal { get; set; }
        public double passengers { get; set; }
    }
}
