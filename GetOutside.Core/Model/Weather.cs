using SQLite;
using System;

namespace GetOutside.Core.Model
{
    public class Weather
    {
        [PrimaryKey, AutoIncrement, Column("WeatherId")]
        public int Temperature { get; set; }
        public string Units { get; set; }
        public string Precipitation { get; set; }

        // track daylight or nighttime
    }
}
