using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlightControlWeb.Models
{
    public class ServerManager : IServerManager
    {
        public Server AddServer(Server server)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Servers (ServerID, ServerURL) values (@ServerID,@ServerURL)", server);
            }
            return server;
        }

        public IEnumerable<Server> GetAllServers()
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Server>("select * from Servers", new DynamicParameters());
                return output;
            }
        }
        public void DeleteServer(string id)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            { 
                cnn.Open();
                // Checking the server exists in the db 
                using var con = new SQLiteConnection(LoadConnectionString());
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Servers WHERE serverID = " +
                    "\"" + id + "\"", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if(!reader.HasRows)
                    throw new System.Data.SQLite.SQLiteException();
                reader.Close();
                SQLiteCommand sqlComm = cnn.CreateCommand();
                string com = "DELETE FROM Servers WHERE ServerID=" + "\"" + id + "\"";
                sqlComm.CommandText = com;
                sqlComm.ExecuteNonQuery();
                cnn.Close();
            }
        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
