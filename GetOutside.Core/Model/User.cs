namespace GetOutside.Database
{
    internal class User
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        //public date DateofBirth { get; set; }
        public int GoalHours { get; set; }

    }
}