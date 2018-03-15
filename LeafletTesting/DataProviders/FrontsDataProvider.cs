using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using LeafletTesting.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace LeafletTesting.Data.MapDataProviders
{
    public interface IFrontsDataProvider
    {
        void generateFrontJsonFiles(string frontDataFilepath, string DataFilePath);
    }

    public class FrontsDataProvider : BaseDataProvider, IFrontsDataProvider
    {

        public void generateFrontJsonFiles(string frontDataFilepath, string DataFilePath)
        {
            var frontFileNames = ConfigurationManager.AppSettings["FrontDataFileNames"];
            var paths = Directory.GetFiles(DataFilePath, frontFileNames);

            List<Feature> mainListFeatures = new List<Feature>();

            foreach (string path in paths)
            {
                var stringData = new List<string>();
                var featureType = "";

                using (var reader = new StreamReader(path))
                {
                    for (int z = 0; z < 2; ++z)
                    {
                        //skip the first 2 lines of the file
                        var skipLine = reader.ReadLine().Trim();
                    }

                    while (!reader.EndOfStream)
                    {
                        List<string> propertiesLineList = new List<string>();
                        List<string> propertiesCoorrdinates = new List<string>();

                        var line = reader.ReadLine().Trim();

                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] tempLine = line.Split(',');
                            int numLatLongPairs = Int16.Parse(tempLine[2]);
                            int endIndex = ((numLatLongPairs * 2) + 3 - 1);

                            var text = "";
                            var width = "";
                            int[] strokeDash;

                            if (tempLine[0] == "symbol_lowp")
                            {
                                text = "L";
                            }
                            else if (tempLine[0] == "symbol_high")
                            {
                                text = "H";
                            }
                            else
                            {
                                if (tempLine[endIndex + 2].Contains("$$"))
                                {
                                    //replacing the '$$' with \n for later in the javascrip
                                    var tempLineText = tempLine[endIndex + 2].Replace("$$", "\n");
                                    text = tempLineText;
                                }
                                else
                                {
                                    //use the tet thats included in the file
                                    text = tempLine[endIndex + 2];
                                }
                            }

                            if (tempLine[endIndex + 3] == "")
                            {
                                width = "3";
                            }
                            else
                            {
                                width = tempLine[endIndex + 3];
                            }

                            if (tempLine[0] == "front_trof_strng")
                            {
                                strokeDash = new int[] { 10, 10 };
                            }
                            else
                            {
                                //create empty array
                                strokeDash = new int[] { };
                            }

                            PointGeometry geometryPoint = new PointGeometry();
                            PolygonGeometry geometryPoly = new PolygonGeometry();
                            var allCoordinates = new List<List<double>>();

                            Feature feature = new Feature();

                            if (numLatLongPairs <= 1)
                            {
                                feature.geometry = new PointGeometry()
                                {
                                    coordinates = new List<double> {
                                        double.Parse(tempLine[4]),
                                        double.Parse(tempLine[3])
                                        }
                                };

                                featureType = "Point";
                            }
                            else
                            {
                                List<List<double>> coordinates = new List<List<double>>();

                                for (int i = 3; i < endIndex; i += 2)
                                {
                                    coordinates.Add( new List<double>
                                    {
                                        double.Parse(tempLine[i + 1]),
                                        double.Parse(tempLine[i])
                                    });
                                }

                                //**************NEW MODEL*********************
                                feature.geometry = new LineStringGeometry()
                                {
                                    coordinates = coordinates
                                };

                                featureType = "LineString";
                            }

                            feature.properties = new FrontsDataProperties
                            {
                                ObjectType = tempLine[0],
                                strokeColor = "#" + tempLine[1].Substring(4),
                                Num_LatLongPairs = tempLine[2],
                                ObjectSize = tempLine[endIndex + 1],
                                Text = text,
                                Width = width,
                                Smoothing = tempLine[endIndex + 4],
                                ClosedLineFlag = tempLine[endIndex + 5],
                                FilledLineFlag = tempLine[endIndex + 6],
                                PipDirection = tempLine[endIndex + 7],
                                Identifier = featureType
                            };

                            mainListFeatures.Add(feature);
                        }

                    }
                    reader.Close();
                }
            }

            using (StreamWriter file = File.CreateText(frontDataFilepath + "FrontsGeoJson.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.None;
                serializer.Serialize(file, new FeatureCollection { name = "Fronts", features = mainListFeatures });
            }
        }
    }
}