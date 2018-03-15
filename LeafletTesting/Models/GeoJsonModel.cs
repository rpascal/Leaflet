using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeafletTesting.Models
{
    public class FeatureCollection
    {
        public string name { get; set; } = "Name";
        public string type { get; set; } = "FeatureCollection";
        public List<Feature> features { get; set; }
    }
    public class Feature
    {
        public string type { get; set; } = "Feature";
        public BaseGeometry geometry { get; set; }
        public BaseProperties properties { get; set; }
    }

    public class BaseGeometry
    {
        public string type { get; set; }
    }
    public class BaseProperties
    {
        public string type { get; set; }
    }

    public class PolygonGeometry : BaseGeometry
    {
        public PolygonGeometry()
        {
            type = "Polygon";
        }

        public List<List<List<double>>> coordinates { get; set; } = new List<List<List<double>>>();
    }

    public class LineStringGeometry : BaseGeometry
    {
        public LineStringGeometry()
        {
            type = "LineString";
        }
        public List<List<double>> coordinates { get; set; } = new List<List<double>>();
    }

    public class PointGeometry : BaseGeometry
    {
        public PointGeometry()
        {
            type = "Point";
        }
        public List<double> coordinates { get; set; } = new List<double>();
    }






    /*NEED SEPERATE POINT MODEL*/
    //public class GeoJsonModelPoint<T> where T : BaseProperties
    //{
    //    public string name { get; set; } = "Name";
    //    public string type { get; set; } = "FeatureCollection";
    //    public List<FeaturePoint<T>> features { get; set; }
    //}
   

    //public class FeaturePoint<T> where T : BaseProperties
    //{
    //    public string type { get; set; } = "Feature";
    //    public GeometryPoint geometry { get; set; }
    //    public T properties { get; set; }
    //}
    /*NEED SEPERATE POINT MODEL*/


    public class WinterDataProperties : BaseProperties
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

    public class WarnDataProperties : BaseProperties
    {
        public string Station { get; set; }
    }

    public class LightningStrikeDataProperties : BaseProperties
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public decimal Polarity { get; set; }
        public int OrderBy { get; set; }
    }

    public class FrontsDataProperties : BaseProperties
    {
        public string ObjectType { get; set; }
        public string strokeColor { get; set; }
        public string Num_LatLongPairs { get; set; }
        public string ObjectSize { get; set; }
        public string Text { get; set; }
        public string Width { get; set; }
        public string Smoothing { get; set; }
        public string ClosedLineFlag { get; set; }
        public string FilledLineFlag { get; set; }
        public string PipDirection { get; set; }
        public string Identifier { get; set; }
    }

}
