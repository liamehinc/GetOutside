using SQLite;
using System;

namespace GetOutside.Database
{
    internal class User
    {
        [PrimaryKey, AutoIncrement, Column("UserId")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        //public date DateofBirth { get; set; }
        public int GoalHours { get; set; }

    }
}