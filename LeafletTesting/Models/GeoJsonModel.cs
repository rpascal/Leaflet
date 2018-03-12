using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Weather.Models
{
    public class GeoJsonModel<T> where T : Properties
    {
        public string name { get; set; } = "Name";
        public string type { get; set; } = "FeatureCollection";
        public List<Feature<T>> features { get; set; }
    }
    public class Geometry
    {
        public string type { get; set; } = "Polygon";
        public List<List<List<double>>> coordinates { get; set; } = new List<List<List<double>>>();
    }

    public class Properties
    {
        public string type { get; set; }
    }

    public class Feature<T> where T : Properties
    {
        public string type { get; set; } = "Feature";
        public Geometry geometry { get; set; }
        public T properties { get; set; }
    }


    /*NEED SEPERATE POINT MODEL*/
    public class GeoJsonModelPoint<T> where T : Properties
    {
        public string name { get; set; } = "Name";
        public string type { get; set; } = "FeatureCollection";
        public List<FeaturePoint<T>> features { get; set; }
    }
    public class GeometryPoint
    {
        public string type { get; set; } = "Point";
        public List<double> coordinates { get; set; } = new List<double>();
    }

    public class FeaturePoint<T> where T : Properties
    {
        public string type { get; set; } = "Feature";
        public GeometryPoint geometry { get; set; }
        public T properties { get; set; }
    }
    /*NEED SEPERATE POINT MODEL*/


    public class WinterDataProperties : Properties
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string strokeColor { get; set; }
        public int strokeThickness { get; set; }
        public string InfoboxTitle { get; set; }
        public string Fips { get; set; }
        public string State { get; set; }
        public string Center { get; set; }
        public string City { get; set; }

    }

    public class WarnDataProperties : Properties
    {
        public string Station { get; set; }
    }

    public class LightningStrikeDataProperties : Properties
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public decimal Polarity { get; set; }
        public int OrderBy { get; set; }
    }
}
