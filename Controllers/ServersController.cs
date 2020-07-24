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
    public class ServersController : ControllerBase
    {

        private readonly IServerManager serverManager;
        public ServersController(IServerManager serverManager1)
        {
            serverManager = serverManager1;
        }

        // GET: api/Servers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Server>> GetServers()
        {
            var servers = serverManager.GetAllServers().ToArray();
            if (servers.Length == 0) // No servers
                return NotFound();
            else
                return Ok(servers);
        }
        
        // POST: api/Servers
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Server> Post(Server server)
        {
            try
            {
                return Ok(serverManager.AddServer(server));
            }
            catch (System.Data.SQLite.SQLiteException) 
            {
                // Id inserted is already in the DB
                return BadRequest("ID Already in DB");
            }
            
        }
        // DELETE: api/Servers/5
        [HttpDelete("{serverId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Delete(string serverId)
        {
            try
            {
                serverManager.DeleteServer(serverId);
                return Ok();
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                return BadRequest("ID not in DB");
            }
            
        }

    }
}
