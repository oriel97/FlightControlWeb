using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private readonly IFlightPlanManager flightPlanManager;
        public FlightPlanController(IFlightPlanManager flightPlanManager1)
        {
            flightPlanManager = flightPlanManager1;
        }        

        // GET: api/FlightPlan/ID
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FlightPlan> GetFlightPlanById(string id)
        {
            var flightPlan = flightPlanManager.GetFlightPlanByID(id).Result;
            if (flightPlan == null)
                return NotFound();
            else
                return Ok(flightPlan);
        }

        // POST: api/FlightPlan
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<FlightPlan> AddFlightPlan(FlightPlan flightPlan)
        {
            return Ok(flightPlanManager.AddFlightPlan(flightPlan));
        }
    }
}
