using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightManager
    {
        Task<List<Flight>> GetAllFlights(DateTime date);
        List<Flight> GetInternalFlights(DateTime date);
        void DeleteFlight(string flightID);
    }
}
