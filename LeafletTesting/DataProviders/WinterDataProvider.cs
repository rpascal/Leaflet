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

            var features = new List<Feature>();


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


                        string[] tempLine = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string fip = tempLine[1];
                        string state = tempLine[3];
                        string city = tempLine[2];
                        string center = tempLine[9];


                        
                        WinterDataProperties properties = new WinterDataProperties
                        {
                            type = propertiesLineList[1],
                            strokeColor = GetBorderColor(propertiesLineList[1], types.WINTER),
                            strokeThickness = GetPolyBorderThickness(propertiesLineList[0], types.WINTER),
                            StartDateTime = ParseDateTime(propertiesLineList[2]),
                            EndDateTime = ParseDateTime(propertiesLineList[3]),
                            InfoboxTitle = GetInfoboxTitle(propertiesLineList[1], types.WINTER),
                            State = state,
                        Center = center,
                       Fips = fip,
                        City = city,

                    };

                        Feature feature = new Feature()
                        {
                            properties = properties
                        };
                        PolygonGeometry geometry = new PolygonGeometry();

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

                        if (features.Exists(x => ((WinterDataProperties)x.properties).Fips.Equals(fip)))
                        {
                            features.Remove(features.Find(x => ((WinterDataProperties)x.properties).Fips.Equals(fip)));
                        }
                        features.Add(feature);

                    }
                };

                List<DateTime> dates = getListOfDates(DataFilePath);


                for (int i = 0; i < dates.Count; i++)
                {
                    DateTime currentDate = dates[i];
                    List<Feature> dateFilteredFeatures = new List<Feature>();

                    if (i == dates.Count - 1)
                    {
                        dateFilteredFeatures = features.Where(x =>
                        {
                            return Convert.ToDateTime(((WinterDataProperties)x.properties).EndDateTime) >= currentDate;
                        }).ToList();
                    }
                    else
                    {
                        DateTime nextDate = dates[i + 1];
                        dateFilteredFeatures = features.Where(x =>
                        {
                            /*
                             Check if endDate is after or equal to current date
                             ALSO
                             check if startDate is BEFORE the next date
                          
                             */
                            return Convert.ToDateTime(((WinterDataProperties)x.properties).EndDateTime) >= currentDate
                            && Convert.ToDateTime(((WinterDataProperties)x.properties).StartDateTime) < nextDate;
                        }).ToList();
                    }



                    using (StreamWriter file = File.CreateText(winterDataFilepath + "winterWarnings_" + i + ".json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.None;
                        serializer.Serialize(file, new FeatureCollection { name = "winterWarnings_" + i, features = dateFilteredFeatures });
                    }
                }
            }
        }
    }
}

