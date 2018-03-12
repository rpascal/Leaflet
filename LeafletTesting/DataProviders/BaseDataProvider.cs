using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Globalization;
using LeafletTesting.Models;
using System.Linq;

namespace LeafletTesting.Data
{
    public class BaseDataProvider
    {

        public static string ParseDateTime(string input)
        {
            string format = "yyMMdd HHmm";
            var stringTemp = input.Replace('/', ' ');
            DateTime dateTimeConvert;

            dateTimeConvert = DateTime.ParseExact(stringTemp, format, CultureInfo.InvariantCulture).ToLocalTime();

            return dateTimeConvert.ToString(); ;
        }

        public static string ParseDateTimeCurCond(string rawData)
        {
            string formatDate = "yyyyMMdd HHmm";
            DateTime dateTimeConvert;
            dateTimeConvert = DateTime.ParseExact(rawData, formatDate, CultureInfo.InvariantCulture).ToLocalTime();

            return dateTimeConvert.ToString(); ;
        }

        //public static List<latLongs> ParseLatLong(string input, types type)
        //{
        //    var numbers = input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
        //                        .Select(i => decimal.Parse(i))
        //                        .ToArray();

        //    var latLonPairs = new List<latLongs>();

        //    switch (type)
        //    {
        //        case types.WATCH:
        //        case types.WINTER:

        //            for (int i = 0; i < numbers.Length; i += 2)
        //            {
        //                latLonPairs.Add(new latLongs
        //                {
        //                    latitude = numbers[i],
        //                    longitude = numbers[i + 1],
        //                });
        //            }

        //            latLonPairs.Add(new latLongs
        //            {
        //                latitude = numbers[0],
        //                longitude = numbers[1],
        //            });

        //            break;
        //        case types.WARN:
        //            for (int i = 0; i < numbers.Length; i += 2)
        //            {
        //                latLonPairs.Add(new latLongs
        //                {
        //                    latitude = (numbers[i]) / 100,
        //                    longitude = (numbers[i + 1]) / -100,
        //                });
        //            }

        //            latLonPairs.Add(new latLongs
        //            {
        //                latitude = (numbers[0]) / 100,
        //                longitude = (numbers[1]) / -100,
        //            });

        //            var newTemp = latLonPairs;
        //            break;
        //    }
        //    return latLonPairs;
        //}

        public static List<DateTime> getListOfDates(string mapPath)
        {
            var dates = new List<DateTime>();

            using (var reader = new StreamReader(mapPath + "/fnames.txt"))
            {
                while (!reader.EndOfStream)
                {
                    var data = reader.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(data))
                    {
                        //var stringTemp = data.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        string format2 = "h:mm tt ddd MMM-dd-yyyy";

                        DateTime dateTime = DateTime.ParseExact(data, format2, CultureInfo.InvariantCulture);

                        dates.Add(dateTime);
                    }
                }
                reader.Close();
            }
            return dates;
        }

        public static int GetPolyBorderThickness(string rawData, types type)
        {
            int polyThick = 2;

            switch (type)
            {
                case types.WATCH:
                    switch (rawData)
                    {
                        case "TOR":
                            polyThick = 4;
                            break;
                        case "SVR":
                            polyThick = 4;
                            break;
                    }
                    break;
                case types.WARN:
                    switch (rawData)
                    {
                        case "TOR":
                            polyThick = 2;
                            break;
                        case "SVR":
                            polyThick = 2;
                            break;
                        case "FFW":
                            polyThick = 2;
                            break;
                    }
                    break;

            }
            return polyThick;
        }

        public static string GetBorderColor(string rawData, types type)
        {
            var polyColor = "";

            switch (type)
            {
                case types.WATCH:
                    switch (rawData)
                    {
                        case "SVR":
                            polyColor = "#ffff00";      //yellow
                            break;
                        case "TN":
                            polyColor = "#ff0000";      //red
                            break;
                    }
                    break;
                case types.WINTER:
                    switch (rawData)
                    {
                        case "WRN":
                            polyColor = "#ffffff";      //white
                            break;
                        case "WTC":
                            polyColor = "#00ffff";      //aqua
                            break;
                        case "ADV":
                            polyColor = "#b200ff";      //electric purple
                            break;
                    }
                    break;
                case types.WARN:
                    switch (rawData)
                    {
                        case "TOR":
                            polyColor = "#ff0000";      //red
                            break;
                        case "SVR":
                            polyColor = "#ffff00";      //yellow
                            break;
                        case "FFW":
                            polyColor = "#00ff21";      //lime green
                            break;
                    }
                    break;
            }
            return polyColor;
        }

        public static string GetInfoboxTitle(string rawData, types type)
        {
            var infoboxTitle = rawData;

            switch (type)
            {
                case types.WATCH:
                    switch (rawData)
                    {
                        case "SVR":
                            infoboxTitle = "Severe T-Storm Watch";
                            break;
                        case "TN":
                            infoboxTitle = "Tornado Watch";
                            break;
                    }
                    break;
                case types.WINTER:
                    switch (rawData)
                    {
                        case "WRN":
                            infoboxTitle = "Winter Weather Warning";
                            break;
                        case "WTC":
                            infoboxTitle = "Winter Weather Watch";
                            break;
                        case "ADV":
                            infoboxTitle = "Winter Weather Advisory";
                            break;
                    }
                    break;
                case types.WARN:
                    switch (rawData)
                    {
                        case "TOR":
                            infoboxTitle = "Tornado Warning";
                            break;
                        case "SVR":
                            infoboxTitle = "Severe T-Storm Warning";
                            break;
                        case "FFW":
                            infoboxTitle = "Flash Flood Warning";
                            break;
                    }
                    break;
            }
            return infoboxTitle;
        }

        public static string getCurCondErrNull(string rawData)
        {
            //checks for nulls and -9999 errors in data and returns empty string if needed
            var retval = rawData;
            switch (rawData)
            {
                case null:
                    retval = "";
                    break;
                case "-9999":
                    retval = "";
                    break;

            }
            return retval;
        }
    }

    public enum types
    {
        WARN, WINTER, WATCH
    }


}
