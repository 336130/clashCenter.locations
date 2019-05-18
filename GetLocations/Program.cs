using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GetLocations
{
    class Program
    {
        static void Main(string[] args)
        {
            var webRequest = HttpWebRequest.Create("https://api.clashofclans.com/v1/locations");
            string locationKey = ConfigurationManager.AppSettings["LocationKey"];
            webRequest.Headers.Add("authorization", "Bearer " + locationKey);

            using (var response = webRequest.GetResponse())
            {
                var dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                var asObject = JsonConvert.DeserializeObject<LocationList>(responseFromServer);
                Console.WriteLine(responseFromServer);
                using (var LocationDBContext = new ClashCenterEntities()) {
                    foreach (var location in asObject.Items)
                    {
                        Location newLoc = new Location
                        {
                            LocationID = location.ID,
                            Name = location.Name,
                            CountryCode = location.CountryCode,
                            IsCountry = location.IsCountry
                        };

                        LocationDBContext.Locations.Add(newLoc);
                    }
                    LocationDBContext.SaveChanges();
                };
                var i = 0;
            }
        }
    }
}
