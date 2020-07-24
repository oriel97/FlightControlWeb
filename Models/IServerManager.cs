using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public interface IServerManager
    {
        IEnumerable<Server> GetAllServers();
        Server AddServer(Server server);
        void DeleteServer(string id);
    }
}
