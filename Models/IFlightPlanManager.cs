using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IFlightPlanManager
    {
        FlightPlan AddFlightPlan(FlightPlan flightPlan);
        Task<FlightPlan> GetFlightPlanByID(string flightID);
    }
}
