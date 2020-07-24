using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyFlightManager : IFlightManager
    {
        public void DeleteFlight(string flightID)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //open the connection
                cnn.Open();

                // delete all the segments of this flight.
                SQLiteCommand sqlComm = cnn.CreateCommand();
                string com = "DELETE FROM Segments WHERE flight_ID=" + "\"" + flightID + "\"";
                sqlComm.CommandText = com;
                sqlComm.ExecuteNonQuery();

                // find the location id to delete the location from the table
                sqlComm = new SQLiteCommand("SELECT * FROM FlightPlans WHERE flight_ID = " + "\"" + flightID + "\"", cnn);
                SQLiteDataReader reader = sqlComm.ExecuteReader();
                reader.Read();
                // if there is no such flight in the database
                if (!reader.HasRows)
                    throw new System.Data.SQLite.SQLiteException();

                string location_ID = reader["location_ID"].ToString();
                reader.Close();

                // delete the location from the table
                sqlComm = cnn.CreateCommand();
                com = "DELETE FROM Locations WHERE id = " + "\"" + location_ID + "\"";
                sqlComm.CommandText = com;
                sqlComm.ExecuteNonQuery();

                // delete the flight plan from the table
                sqlComm = cnn.CreateCommand();
                com = "DELETE FROM FlightPlans WHERE flight_ID=" + "\"" + flightID + "\"";
                sqlComm.CommandText = com;
                sqlComm.ExecuteNonQuery();

                // close the connection
                cnn.Close();
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public async Task<List<Flight>> GetAllFlights(DateTime date)
        {
            string date1 = date.ToString("yyyy-MM-ddTHH':'mm':'ss") + "Z";
            Debug.WriteLine(date1);
            List<Flight> list = GetInternalFlights(date);
            //add to list from external servers
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //open the connection
                cnn.Open();

                SQLiteCommand sqlComm = cnn.CreateCommand();
                sqlComm = new SQLiteCommand("SELECT * FROM Servers", cnn);
                SQLiteDataReader reader = sqlComm.ExecuteReader();
                MyFlightPlanManager planManager = new MyFlightPlanManager();
                while (reader.Read()) // iterating on all the servers
                {
                    Server server = new Server { 
                        ServerID = reader["ServerID"].ToString(), 
                        ServerURL = reader["ServerURL"].ToString() 
                    };

                    List<Flight> externals = new List<Flight>();
                    HttpClient client = new HttpClient();

                    try
                    {
                        string msg = server.ServerURL + "/api/Flights?relative_to=" + date1;
                        HttpResponseMessage response = await client.GetAsync(msg);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        Debug.WriteLine(responseBody);
                        try
                        {
                            if (response.IsSuccessStatusCode && !responseBody.Contains("fail"))
                                externals = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Flight>>(responseBody);
                            else
                                Debug.WriteLine("\nError in getting flights from server");
                        }
                        catch (JsonReaderException e1)
                        {
                            Console.WriteLine("\nException Caught!" + e1);
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }

                    client.Dispose();

                    foreach (Flight f in externals.ToList())
                    {
                        f.isExternal = true;
                    }
                    list = list.Concat(externals).ToList();
                }
            }
            return list;

        }

        public List<Flight> GetInternalFlights(DateTime dateTime)
        {
            List<Flight> internals = new List<Flight>();
            using (SQLiteConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                //open the connection
                cnn.Open();

                SQLiteCommand sqlComm = cnn.CreateCommand();
                sqlComm = new SQLiteCommand("SELECT * FROM FlightPlans", cnn);
                SQLiteDataReader reader = sqlComm.ExecuteReader();
                MyFlightPlanManager planManager = new MyFlightPlanManager();
                // For each flightPlan in the database, we will create p1 Flight object
                while (reader.Read()) 
                {
                    string flightPlanID = reader["flight_id"].ToString();
                    FlightPlan flightPlan = planManager.GetFlightPlanByID(flightPlanID).Result;
                    if (OnTime(flightPlan, dateTime))
                    {
                        InitialLocation flightLocation = GetPosition(flightPlan, dateTime);
                        DateTime dateTime1 = dateTime;
                        string date1 = dateTime1.ToString();
                        internals.Add(new Flight()
                        {
                            passengers = flightPlan.passengers,
                            flight_id = flightPlanID,
                            Company_name = flightPlan.company_name,
                            isExternal = false,
                            Longitude = flightLocation.longitude,
                            Latitude = flightLocation.latitude,
                            date_time = date1
                        });
                    }
                }
                reader.Close();
            }
            return internals;
        }
        public bool OnTime(FlightPlan fp, DateTime dateTime)
        {
            DateTime liftOffTime = DateTime.Parse(fp.initial_location.date_time);
            DateTime landingTime = DateTime.Parse(fp.initial_location.date_time);
            liftOffTime = liftOffTime.ToUniversalTime();
            landingTime = landingTime.ToUniversalTime();
            foreach (Segment s in fp.segments)
            {
                landingTime = landingTime.AddSeconds(s.timespan_seconds);
            }
            if (dateTime.CompareTo(liftOffTime) >= 0 && dateTime.CompareTo(landingTime) <= 0)
            {
                return true;
            }
            return false;
        }

        public InitialLocation GetPosition(FlightPlan fp, DateTime dateTime)
        {
            DateTime p1 = DateTime.Parse(fp.initial_location.date_time);
            p1 = p1.ToUniversalTime();
            DateTime p2;
            Segment segmentA = new Segment {
                longitude = fp.initial_location.longitude, 
                latitude = fp.initial_location.latitude, 
                timespan_seconds = 0 };
            Segment segmentB;

            foreach (Segment s in fp.segments)
            {
                p2 = p1;
                p2 = p2.AddSeconds(s.timespan_seconds);
                segmentB = s;

                if (dateTime.CompareTo(p1) >= 0 && dateTime.CompareTo(p2) <= 0)
                {
                    TimeSpan secsInFlightFromA = dateTime.Subtract(p1);
                    double ApproxTimeInFlight = secsInFlightFromA.TotalSeconds / s.timespan_seconds;

                    double startLong = segmentA.longitude;
                    double startLat = segmentA.latitude;

                    double endLong = segmentB.longitude;
                    double endLat = segmentB.latitude;

                    double ansLongitude = segmentA.longitude + ApproxTimeInFlight * (endLong - startLong) ;
                    double ansLatitude = segmentA.latitude + ApproxTimeInFlight * (endLat - startLat);

                    return new InitialLocation {
                        longitude = ansLongitude,
                        latitude = ansLatitude,
                        date_time = dateTime.ToString() 
                    };
                }
                segmentA = segmentB;
                p1 = p2;
            }
            return null;
        }

    }
}
