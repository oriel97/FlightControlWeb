using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyFlightPlanManager : IFlightPlanManager
    {
        public FlightPlan AddFlightPlan(FlightPlan flightPlan)
        {
            // Creating a new flight ID and validating it
            string id = GenerateRandomId();
            InitialLocation loc = flightPlan.initial_location;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // Inserting the location
                cnn.Execute("insert into Locations (longitude,latitude,date) values " +
                    "(@longitude,@latitude,@date_time)", loc);
                string stm = "SELECT * FROM Locations ORDER BY id DESC LIMIT 1";
                using var con = new SQLiteConnection(LoadConnectionString());
                con.Open();
                using var cmd = new SQLiteCommand(stm, con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                string id1 = "";
                id1 = reader["id"].ToString();
                reader.Close();

                // Inserting the FlightPlan
                cmd.CommandText =
                    "INSERT INTO FlightPlans(flight_ID,company_name,passengers,location_id) " +
                    "VALUES(@flight_ID,@company_name,@passengers,@location_id)";
                cmd.Parameters.AddWithValue("@flight_ID", id);
                cmd.Parameters.AddWithValue("@company_name", flightPlan.company_name);
                cmd.Parameters.AddWithValue("@passengers", flightPlan.passengers);
                cmd.Parameters.AddWithValue("@location_id", id1);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

                // Inserting Segments
                foreach (var segment in flightPlan.segments)
                {
                    cmd.CommandText =
                        "INSERT INTO Segments(longitude,latitude,timespan_seconds,flight_ID) " +
                    "VALUES(@longitude,@latitude,@timespan_seconds,@flight_ID)";
                    cmd.Parameters.AddWithValue("@longitude",segment.longitude);
                    cmd.Parameters.AddWithValue("@latitude", segment.latitude) ;
                    cmd.Parameters.AddWithValue("@timespan_seconds", segment.timespan_seconds);
                    cmd.Parameters.AddWithValue("@flight_ID", id);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
            return flightPlan;
        }

        public async Task<FlightPlan> GetFlightPlanByID(string flightID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                // If we get the ID with {}
                if (flightID.Contains('{'))
                {
                    int k = flightID.Length - 1;
                    flightID = flightID.Remove(flightID.Length - 1);
                    flightID = flightID.Remove(0, 1);
                }
                // Checking if the Flight is in the DB
                using var con = new SQLiteConnection(LoadConnectionString());
                con.Open();
                SQLiteCommand cmd = 
                    new SQLiteCommand("SELECT * FROM FlightPlans WHERE flight_ID = " +
                    "\"" + flightID + "\"", con);
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (reader.HasRows) // The FlightPlan we want is in the database
                {
                    // When we are asked to retrieve a flight plan, we will fetch it from the data base
                    string company_name = reader["company_name"].ToString();
                    int passengers = Int32.Parse(reader["passengers"].ToString());
                    string location_ID = reader["location_ID"].ToString();
                    reader.Close();

                    // Retrieving the the location
                    cmd = new SQLiteCommand("SELECT * FROM Locations WHERE id = " +
                        "\"" + location_ID + "\"", con);
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    InitialLocation initialLocation = new InitialLocation
                    {
                        longitude = Double.Parse(reader["longitude"].ToString()),
                        latitude = Double.Parse(reader["latitude"].ToString()),
                        date_time = reader["date"].ToString()
                    };
                    reader.Close();

                    // Retrieving the segments
                    List<Segment> segments = new List<Segment>();
                    cmd = new SQLiteCommand("SELECT * FROM Segments WHERE flight_ID = " +
                        "\"" + flightID + "\"", con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        segments.Add(new Segment()
                        {
                            longitude = Double.Parse(reader["longitude"].ToString()),
                            latitude = Double.Parse(reader["latitude"].ToString()),
                            timespan_seconds = Double.Parse(reader["timespan_seconds"].ToString())
                        });
                    }

                    // Creating the new Flight Plan from the data we gathered
                    return new FlightPlan()
                    {
                        company_name = company_name,
                        passengers = passengers,
                        initial_location = initialLocation,
                        segments = segments
                    };
                }
                else // If the flight is not in the database, we will check if its in any of the servers
                {
                    SQLiteCommand sqlComm = con.CreateCommand();
                    sqlComm = new SQLiteCommand("SELECT * FROM Servers", con);
                    reader = sqlComm.ExecuteReader();
                    MyFlightPlanManager planManager = new MyFlightPlanManager();
                    if (reader.HasRows) // there are servers
                    {
                        while (reader.Read()) // iterating on all the servers
                        {
                            Server server = new Server
                            {
                                ServerID = reader["ServerID"].ToString(),
                                ServerURL = reader["ServerURL"].ToString()
                            };
                            HttpClient client = new HttpClient();
                            try
                            {
                                string msg = server.ServerURL + "/api/FlightPlan/" + flightID;
                                HttpResponseMessage response = await client.GetAsync(msg);
                                response.EnsureSuccessStatusCode();
                                string responseBody = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode && !responseBody.Contains("fail"))
                                    return Newtonsoft.Json.JsonConvert.DeserializeObject<FlightPlan>(responseBody);
                                else
                                    continue; // the flightplan is not in this server

                            }
                            catch (HttpRequestException e)
                            {
                                Console.WriteLine("\nException Caught!");
                                Console.WriteLine("Message :{0} ", e.Message);
                            }
                            client.Dispose();
                            
                        }
                        // There is no flight with that ID in the servers
                        return null;
                    }
                    else // no servers - the flight id requested is not in the db nor the servers
                    {
                        return null;
                    }
                }
            }
        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        public string GenerateRandomId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < 2; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            for (int i = 2; i < 6; i++)
            {
                stringChars[i] = numbers[random.Next(numbers.Length)];
            }
            for (int i = 6; i < 8; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
    }

}
