using SQLite;
using System;

namespace GetOutside.Database
{
    public class User
    {
        [PrimaryKey, AutoIncrement, Column("UserId")]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        //public date DateofBirth { get; set; }
        public int GoalHours { get; set; }
        public bool DefaultUser { get; set; }

        public User()
        {
            FirstName = "default";
            LastName = "user";
            GoalHours = 1000;
        }
    }
}
