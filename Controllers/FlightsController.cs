using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightManager flightManager;
        public FlightsController(IFlightManager flightManager1)
        {
            flightManager = flightManager1;
        }

        // GET: api/Flights
        [HttpGet]   
        public ActionResult<List<Flight>> GetAllFlights([FromQuery(Name = "relative_to")] DateTime time)
        {
            time = time.ToUniversalTime();
            if (Request.Query.ContainsKey("sync_all") || Request.QueryString.ToString().Contains("sync_all"))
            {
                var helper = flightManager.GetAllFlights(time).Result;
                if (helper.Count == 0) // Making sure there are flights
                    return NotFound();
                else
                    return Ok(helper);
            }
            var helper1 = flightManager.GetInternalFlights(time);
            if (helper1.Count == 0) // Making sure there are flights
                return NotFound();
            else
                return Ok(helper1);
        }


        // DELETE: api/Flights/id
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete(string id)
        {
            try
            {
                flightManager.DeleteFlight(id);
                return Ok();
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                return BadRequest("Id not in DB");
            }
        }
    }
}
