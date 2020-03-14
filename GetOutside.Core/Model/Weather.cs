using System;
using System.Collections.Generic;
using System.Text;

namespace GetOutside.Core.Model
{
    public class Weather
    {
        public int Temperature { get; set; }
        public string Units { get; set; }
        public string Precipitation { get; set; }

        // track daylight or nighttime
    }
}
