using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LeafletTesting.Models.BoundsJsonModel;
using Newtonsoft;
using Newtonsoft.Json;

namespace LeafletTesting.Data.MapDataProviders
{
    public interface ICreateBoundsJson
    {
        List<WinterJsonModel> getWinterJson(string winterDataFilePath, string DataFilePath);
    }

    public class CreateBoundsJsonProvider : BaseDataProvider, ICreateBoundsJson
    {
        public List<WinterJsonModel> getWinterJson(string winterDataFilePath, string DataFilePath)
        {
            var jsonPath = winterDataFilePath + ConfigurationManager.AppSettings["WinterBoundsJson"];

            if (!File.Exists(jsonPath))
            {
                //Debug.WriteLine("JSON File does not exist");
                //we are checking to see if a file exists in order to create new json files if needed.  We would need to create a new json file if the bounds
                //table that is received from the NWS/NOAA changes.  in order to update the json file, delete winterBounds.json and winterBounds-Formatted.json
                //from /weather/classes directory
                //As soon as the application runs again, we will chek to see if winterBounds.json exits.  if it does then we move on to getJson.
                //if it does not, we move to createWinterjson to create the files and then the process continues as normal

                createWinterJson(DataFilePath);
            }

            return getJson(jsonPath);
        }

        public List<WinterJsonModel> getJson(string jsonPath)
        {
            List<WinterJsonModel> returnResult = new List<WinterJsonModel>();

            using (StreamReader file = File.OpenText(jsonPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<WinterJsonModel> deserializeJson = (List<WinterJsonModel>)serializer.Deserialize(file, typeof(List<WinterJsonModel>));
                returnResult = deserializeJson;
            }

            return returnResult;
        }

        private void createWinterJson(string mapPath)
        {
            var winterBoundsPath = mapPath + ConfigurationManager.AppSettings["PFZBoundsFileName"];

            List<WinterJsonModel> winterFips = new List<WinterJsonModel>();
            //PolyCoords coords = null;
            WinterJsonModel fipGroups = new WinterJsonModel();

            string currentFip = "";
            string currentState = "";
            string currentCenter = "";

            using (var reader = new StreamReader(winterBoundsPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    if (line.Length > 0 && !Char.IsLetter(line[0]))
                    {
                        if (line.StartsWith("<FIPS>"))
                        {
                            string[] rawData1 = line.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

                            currentFip = rawData1[1];
                            currentState = rawData1[3];
                            currentCenter = rawData1[5];
                        }
                        else
                        {
                            string[] rawData2 = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToArray();//.ToString();//.Select(x => decimal.Parse(x)).ToArray();
                            //if odd number of values in a line
                            if (rawData2.Count() % 2 == 1)
                            {
                                if (!string.IsNullOrWhiteSpace(fipGroups.FIPS) && fipGroups.LatLongPrs.Count > 2)
                                    winterFips.Add(fipGroups);


                                fipGroups = new WinterJsonModel()
                                {
                                    Center = currentCenter,
                                    State = currentState,
                                    FIPS = currentFip
                                };

                                //coords = new PolyCoords();
                                fipGroups.LatLongPrs.Add(new latLongsJson()
                                {
                                    latitude = Convert.ToDecimal(rawData2[5]),
                                    longitude = Convert.ToDecimal(rawData2[6])
                                });
                            }
                            else
                            {
                                string[] rawArray = rawData2;

                                for (int i = 0; i < rawData2.Length; i += 2)
                                {
                                    fipGroups.LatLongPrs.Add(new latLongsJson()
                                    {
                                        latitude = Convert.ToDecimal(rawArray[i]),
                                        longitude = Convert.ToDecimal(rawArray[i + 1])
                                    });
                                }

                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(fipGroups.FIPS) && fipGroups.LatLongPrs.Count > 2)
                {
                    winterFips.Add(fipGroups);
                }

            }

            string json = JsonConvert.SerializeObject(winterFips, Formatting.Indented);

            // serialize JSON directly to a file -minimized for performance
            using (StreamWriter file = File.CreateText(mapPath + "winterBounds.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.None;
                serializer.Serialize(file, winterFips);
            }

            // serialize JSON directly to a file - formatted 
            using (StreamWriter file = File.CreateText(mapPath + "winterBounds-Formatted.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, winterFips);
            }
            var test = json;
        }
    }
}
