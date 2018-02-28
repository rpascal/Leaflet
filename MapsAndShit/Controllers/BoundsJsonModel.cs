using System.Collections.Generic;

namespace LeafletTesting.Models
{
    public class BoundsJsonModel
    {
        public class WinterJsonModel
        {
            public string FIPS { get; set; }
            public string State { get; set; }
            public string Center { get; set; }
            public List<latLongsJson> LatLongPrs { get; set; } = new List<latLongsJson>();

        }

        public class latLongsJson
        {
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
        }

    }
}
