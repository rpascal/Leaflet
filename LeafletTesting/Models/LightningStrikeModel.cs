using System.Collections.Generic;

namespace FE_Weather.Models
{
    public class LightningStrikeModel
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Polarity { get; set; }
        public int OrderBy { get; set; }
    }

}
