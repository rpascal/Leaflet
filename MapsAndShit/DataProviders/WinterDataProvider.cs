using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using LeafletTesting.Models;
using System.Text.RegularExpressions;
using static LeafletTesting.Models.BoundsJsonModel;
using Newtonsoft.Json;

namespace LeafletTesting.Data.MapDataProviders
{
    public interface IWinterDataProvider
    {
        void generateWinterJsonFiles(string winterDataFilepath, string DataFilePath);
    }

    public class WinterDataProvider : BaseDataProvider, IWinterDataProvider
    {
        private readonly ICreateBoundsJson _providerCreateBoundsJson;

        public WinterDataProvider()
        {
            _providerCreateBoundsJson = new CreateBoundsJsonProvider();
        }

        public void generateWinterJsonFiles(string winterDataFilepath, string DataFilePath)
        {
            var winterFileNames = ConfigurationManager.AppSettings["WinterFileNames"];
            var paths = Directory.GetFiles(DataFilePath, winterFileNames);

            List<WinterJsonModel> fipData = _providerCreateBoundsJson.getWinterJson(winterDataFilepath, DataFilePath);
            //List<GeoJsonModel<WinterDataProperties>> winterReturnResult = new List<GeoJsonModel<WinterDataProperties>>();

            var stringData = new List<string>();
            var mainListFeatures = new List<Feature<WinterDataProperties>>();


            var tempCount = 0;

            foreach (string path in paths)
            {
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var data = reader.ReadLine().Trim();

                        if (!string.IsNullOrEmpty(data))
                        {
                            stringData.Add(data);
                        }
                    }
                    reader.Close();
                }
            }

            if (stringData.Count > 0)
            {
                WinterDataProperties tempProperties = null;
                Feature<WinterDataProperties> tempFeature = new Feature<WinterDataProperties>();
                Geometry tempGeometry = new Geometry();

                foreach (string line in stringData)
                {
                    if(line.Contains("320620")) {
                        var i = 1;
                    }

                    if (line.StartsWith("|"))
                    {
                        tempCount++;
                        string[] rawData = line.Split('|');

                        tempProperties = new WinterDataProperties
                        {
                            type = rawData[1],
                            PolyBorderColor = GetBorderColor(rawData[1], types.WINTER),
                            PolyBorderThickness = GetPolyBorderThickness(rawData[0], types.WINTER),
                            StartDateTime = ParseDateTime(rawData[2]),
                            EndDateTime = ParseDateTime(rawData[3]),
                            InfoboxTitle = GetInfoboxTitle(rawData[1], types.WINTER)
                        };
                    }
                    else
                    {
                        tempGeometry = new Geometry();

                        string[] tempLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string fip = tempLine[1];
                        string state = tempLine[3];
                        string center = tempLine[9];

                        // if (fip == "320620")
                        //{

                            tempFeature = new Feature<WinterDataProperties>()
                            {
                                properties = tempProperties
                            };

                            tempFeature.properties.State = state;
                            tempFeature.properties.Center = center;
                            tempFeature.properties.Fips = fip;

                            var coordinates = fipData.Where(x => x.FIPS == fip).Select(s =>
                            {
                                return s.LatLongPrs.Select(q => new List<double> { (double)q.longitude, (double)q.latitude }).ToList();
                            }).ToList();

                            var allCoordinates = new List<List<double>>();

                            coordinates.ForEach(item =>
                            {
                                allCoordinates.AddRange(item);
                            });

                            tempGeometry.coordinates.Add(allCoordinates);

                            if (fip.Contains("320620"))
                            {
                                var i = 1;
                            }

                            tempFeature.geometry = tempGeometry;
                            mainListFeatures.Add(tempFeature);
                        //}
                    }
                };


                var dataTemp1 = mainListFeatures.Where(x => x.properties.Fips.Equals("320620")).ToList();
                                            

                List<DateTime> dates = new List<DateTime>();
                dates = getListOfDates(DataFilePath);

                var dataTemp = mainListFeatures.Where(x => x.properties.Fips.Equals("320620") && Convert.ToDateTime(x.properties.EndDateTime) >= Convert.ToDateTime(dates[0]))
                                            .OrderBy(x => Convert.ToDateTime(x.properties.EndDateTime)).ToList();

                for (int i = 0; i < dates.Count; i++)
                {
                    List<Feature<WinterDataProperties>> tempWintModel = new List<Feature<WinterDataProperties>>();

                    dataTemp.ForEach(item =>
                    {
                        DateTime endDate = Convert.ToDateTime(item.properties.EndDateTime);
                        DateTime startDate = Convert.ToDateTime(item.properties.StartDateTime);

                        if (i == dates.Count - 1)
                        {
                            //denotes last datetime record
                            if (endDate >= dates[i])
                            {
                                tempWintModel.Add(item);
                            }
                        }
                        else
                        {
                            //  add to tempWintModel if the end date falls between 2 datetimes
                            if (endDate >= dates[i] && endDate <= dates[i + 1])
                            {
                                tempWintModel.Add(item);
                            }
                            //  if it does not fall between 2 datetimes, check to see if it is >= current datetime and add
                            else if (endDate >= dates[i])
                            {
                                tempWintModel.Add(item);
                            }
                        }
                    });

                    using (StreamWriter file = File.CreateText(winterDataFilepath + "winterWarnings_" + i + ".json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.None;
                        serializer.Serialize(file, new GeoJsonModel<WinterDataProperties> { name = "winterWarnings_" + i, features = tempWintModel });
                    }
                }
            }
        }
    }
}

