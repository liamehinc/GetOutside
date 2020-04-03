﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetOutside.Core.Model
{
    public class outsideActivity
    {
        [PrimaryKey, AutoIncrement]
        public int OutsideActivityId { get; set; }
        public string Name { get; set; }
        public string YearMonth { get; set; }
        //public int Hours { get; set; }
        //public int Minutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long DurationMilliseconds { get; set; }
        //public Weather Weather { get; set; }
        public string ActivityType { get; set; }
        public int UserId { get; set; }
        public string Notes { get; set; }
        public bool Done { get; set; }

    }
}
