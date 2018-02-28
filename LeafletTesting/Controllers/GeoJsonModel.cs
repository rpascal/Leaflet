using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafletTesting.Models
{
    //public class GeoJsonModel
    //{
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


    public class WinterDataProperties : Properties
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string PolyBorderColor { get; set; }
        public int PolyBorderThickness { get; set; }
        public string InfoboxTitle { get; set; }
        public string Fips { get; set; }
        public string State { get; set; }
        public string Center { get; set; }
        public string city { get; set; }

    }



}
//}
