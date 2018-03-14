using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using FE_Weather.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FE_Weather.Data.MapDataProviders
{
    public interface ILightningDataProvider
    {
        void generateLightningJsonFiles(string lightningDataFilepath, string DataFilePath);
    }

    public class LightningDataProvider : ILightningDataProvider
    {

        public void generateLightningJsonFiles(string lightningDataFilepath, string DataFilePath)
        {
            var lightningFileNames = ConfigurationManager.AppSettings["LightningDataFileNames"];
            var paths = Directory.GetFiles(DataFilePath, lightningFileNames);


            for (var i =0; i< paths.Length; i++)
            {
                var path = paths[i];

                List<FeaturePoint<LightningStrikeDataProperties>> mainListFeatures = new List<FeaturePoint<LightningStrikeDataProperties>>();

                var stringData = new List<string>();

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


                if (stringData.Count > 0)
                {
                    List<string> propertiesLineList = new List<string>();
                    List<string> propertiesCoorrdinates = new List<string>();

                    foreach (string line in stringData)
                    {

                        propertiesLineList = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        var lon = Double.Parse(propertiesLineList[2]);
                        var lat = Double.Parse(propertiesLineList[3]);

                        FeaturePoint<LightningStrikeDataProperties> feature = new FeaturePoint<LightningStrikeDataProperties>()
                        {
                            properties = new LightningStrikeDataProperties
                            {
                                Date = propertiesLineList[0],
                                Polarity = Convert.ToDecimal(propertiesLineList[4])
                            },
                            geometry = new GeometryPoint() {
                                coordinates = new List<double> {  lat, lon  }
                            }                            
                        };

                       mainListFeatures.Add(feature);
                    }
                }


                using (StreamWriter file = File.CreateText(lightningDataFilepath + "ltg_" + (i+1)+ ".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.None;
                    serializer.Serialize(file, new GeoJsonModelPoint<LightningStrikeDataProperties> { name = "Lightning_", features = mainListFeatures });
                }

            }

        }
    }
}
