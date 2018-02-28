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

            var stringData = new List<string>();
            var mainListFeatures = new List<Feature<WinterDataProperties>>();



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
                string testingFip = "320570";
                List<string> propertiesLineList = new List<string>();

                foreach (string line in stringData)
                {

                    if (line.StartsWith("|"))
                    {
                        propertiesLineList = line.Split('|').ToList();
                        /*
                        Here we were doing tempProperites = ...
                        that was the issue we needed to move it inside the else statement
                        because while looping we were setting variables within tempProperties 
                        AND multiple items in the array were pointing to the SAME REFERENCE of tempProperties
                        so when feature.properties.State = state; was called you were actually setting that
                        for each item in the main list which had a reference to that samew tempProperties
                        therefore we made a dumb mistake like i said
                        */
                    }
                    else
                    {
                        WinterDataProperties properties = new WinterDataProperties
                        {
                            type = propertiesLineList[1],
                            PolyBorderColor = GetBorderColor(propertiesLineList[1], types.WINTER),
                            PolyBorderThickness = GetPolyBorderThickness(propertiesLineList[0], types.WINTER),
                            StartDateTime = ParseDateTime(propertiesLineList[2]),
                            EndDateTime = ParseDateTime(propertiesLineList[3]),
                            InfoboxTitle = GetInfoboxTitle(propertiesLineList[1], types.WINTER)
                        };

                        Feature<WinterDataProperties> feature = new Feature<WinterDataProperties>() {
                            properties = properties
                        };
                        Geometry geometry = new Geometry();


                        string[] tempLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string fip = tempLine[1];
                        string state = tempLine[3];
                        string city = tempLine[2];
                        string center = tempLine[9];


                        feature.properties.State = state;
                        feature.properties.Center = center;
                        feature.properties.Fips = fip;
                        feature.properties.city = city;


                        var coordinates = fipData.Where(x => x.FIPS == fip).Select(s =>
                        {
                            return s.LatLongPrs.Select(q => new List<double> { (double)q.longitude, (double)q.latitude }).ToList();
                        }).ToList();

                        var allCoordinates = new List<List<double>>();

                        coordinates.ForEach(item =>
                        {
                            allCoordinates.AddRange(item);
                        });

                        geometry.coordinates.Add(allCoordinates);

                        feature.geometry = geometry;
                        mainListFeatures.Add(feature);

                    }
                };

                var dataTemp1 = mainListFeatures.Where(x => x.properties.Fips.Equals(testingFip)).ToList();

                List<DateTime> dates = new List<DateTime>();
                dates = getListOfDates(DataFilePath);

                var dataTemp = mainListFeatures.Where(x => x.properties.Fips.Equals(testingFip) && Convert.ToDateTime(x.properties.EndDateTime) >= Convert.ToDateTime(dates[0]))
                                            .OrderBy(x => Convert.ToDateTime(x.properties.EndDateTime)).ToList();

                for (int i = 0; i < dates.Count; i++)
                {
                    List<Feature<WinterDataProperties>> dateFilteredFeatures = new List<Feature<WinterDataProperties>>();

                    dataTemp.ForEach(item =>
                    {
                        DateTime endDate = Convert.ToDateTime(item.properties.EndDateTime);
                        DateTime startDate = Convert.ToDateTime(item.properties.StartDateTime);

                        if (i == dates.Count - 1)
                        {
                            //denotes last datetime record
                            if (endDate >= dates[i])
                            {
                                dateFilteredFeatures.Add(item);
                            }
                        }
                        else
                        {
                            //  add to tempWintModel if the end date falls between 2 datetimes
                            if (endDate >= dates[i] && endDate <= dates[i + 1])
                            {
                                dateFilteredFeatures.Add(item);
                            }
                            //  if it does not fall between 2 datetimes, check to see if it is >= current datetime and add
                            else if (endDate >= dates[i])
                            {
                                dateFilteredFeatures.Add(item);
                            }
                        }
                    });

                    using (StreamWriter file = File.CreateText(winterDataFilepath + "winterWarnings_" + i + ".json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.None;
                        serializer.Serialize(file, new GeoJsonModel<WinterDataProperties> { name = "winterWarnings_" + i, features = dateFilteredFeatures });
                    }
                }
            }
        }
    }
}

