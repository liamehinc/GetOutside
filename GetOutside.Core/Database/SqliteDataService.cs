using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Widget;
using GetOutside.Core.Model;
using SQLite;

namespace GetOutside.Database
{
    public sealed class SqliteDataService : IILocalDataService, IDisposable
    {
        private SQLiteConnection _database;

        public void CreateOutsideActivity(OutsideActivity outsideActivity)
        {
            _database.Insert(outsideActivity);
        }

        public void DeleteOutsideActivity(OutsideActivity outsideActivity)
        {
            _database.Delete(outsideActivity);
        }

        public List<OutsideActivity> GetOutsideActivity()
        {
            System.Collections.Generic.List<OutsideActivity> outsideActivities = new System.Collections.Generic.List<OutsideActivity>();
            //            return _database.Table<outsideActivity>().ToList();
            string getOutsideActivityQuery = string.Format(CultureInfo.CurrentCulture, "select OutsideActivityId, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, isTracking from outsideActivity order by StartTime desc");
            outsideActivities = _database.Query<OutsideActivity>(getOutsideActivityQuery);
            return outsideActivities;
        }

        public OutsideActivity GetOutsideActivity(int id)
        {
            string getOutsideActivityQuery = string.Format(CultureInfo.CurrentCulture, "select OutsideActivityId, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, isTracking from outsideActivity where outsideActivityId = {0} limit 1", id);
            List<OutsideActivity> outsideActivities = _database.Query<OutsideActivity>(getOutsideActivityQuery);
            OutsideActivity retreivedOutsideActivity = outsideActivities[0];

            return retreivedOutsideActivity;
        }

        public OutsideActivity GetLatestOutsideActivity()
        {
            string query = "select OutsideActivityId, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, isTracking from outsideActivity order by StartTime desc limit 1";
            System.Collections.Generic.List<OutsideActivity> outsideActivities = _database.Query<OutsideActivity>(query);

            if (outsideActivities != null && outsideActivities.Count > 0)
            {
                return outsideActivities[0];
            }
            else
            {
                return new OutsideActivity();
            }
        }

        public System.Collections.Generic.List<OutsideActivity> GetOutsideHoursByMonth()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds from outsideActivity group by strftime('%Y-%m',date(StartTime/10000000 - 62135596800, 'unixepoch'))  order by StartTime desc";
            System.Collections.Generic.List<OutsideActivity> outsideHoursByMonth = _database.Query<OutsideActivity>(query);
            return outsideHoursByMonth;
        }

        public System.Collections.Generic.List<OutsideActivity> GetOutsideHoursByDay()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds from outsideActivity group by strftime('%Y-%m-%d',date(StartTime/10000000 - 62135596800, 'unixepoch')) order by StartTime desc";
            System.Collections.Generic.List<OutsideActivity> outsideHoursByDay = _database.Query<OutsideActivity>(query);

            return outsideHoursByDay;
        }

        public TimeSpan GetOutsideHours()
        {
            try
            {
                Int64 outsideTime = _database.ExecuteScalar<Int64>("Select SUM(DurationMilliseconds) FROM OutsideActivity where DurationMilliseconds > 0");
                //Int64 outsideTime = _database.Query<int>("Select SUM(DurationMilliseconds) FROM OutsideActivity where DurationMilliseconds > 0");
                DateTime dt = new DateTime(outsideTime);
                long ticks = dt.Ticks * 10000;

                return new TimeSpan(ticks);
            }
            catch (System.NullReferenceException)
            {
                //Toast.MakeText(Application.Context, "There are no activities saved yet", ToastLength.Short).Show();
                return new TimeSpan(0);
            }
        }
        public void Initialize()
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "GetOutsideDatabase.db3");
                _database = new SQLiteConnection(dbPath);
            }

            if (_database.ExecuteScalar<short>("select count(sql) from sqlite_master where name = 'User' and sql like '%integer primary key autoincrement not null%'") == 0)
            {
                _database.DropTable<User>();
            }
            // Create the user table if necessary
            // if the table is created, add a default user
            var createUserTableResult = _database.CreateTable<User>();

            List<User> allUsers = _database.Table<User>().ToList();

            // if there are no users in the table create a default user profile and associate the profile with all activities
            if (allUsers.Count == 0)
            {
                User newUser = new User();
                newUser.DefaultUser = true;

                _database.Insert(newUser);
            }

            // associate default user to unassociated activities
            if(_database.ExecuteScalar<short>("select count(*) from outsideActivity where UserId = 0") > 0)
            {
                int defaultUserId = _database.ExecuteScalar<short>("select UserId from User where defaultUser = 1");
                var setDefaultUserResult = _database.Query<User>(string.Format(CultureInfo.CurrentCulture, "update outsideActivity set UserId = {0}", defaultUserId));
            }

            // check if outsideActivity table auto populates outsideActivityId column. If not, re-create table with auto populate outsideActivityId column
            if (_database.ExecuteScalar<short>("select count(sql) from sqlite_master where name = 'outsideActivity' and sql like '%integer primary key autoincrement not null%'") == 0)
            { 
                string sqlQuery = "BEGIN TRANSACTION; CREATE TEMPORARY TABLE OutsideActivity_backup(Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, IsTracking, IsPaused); INSERT INTO OutsideActivity_backup SELECT (Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, IsTracking, IsPaused) FROM OutsideActivity; DROP TABLE OutsideActivity; CREATE TABLE OutsideActivity(OutsideActivityId INTEGER PRIMARY KEY, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, IsTracking, IsPaused); INSERT INTO OutsideActivity SELECT (Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, IsTracking, IsPaused) FROM OutsideActivity_backup; DROP TABLE OutsideActivity_backup; COMMIT; ";
                _database.Query<OutsideActivity>(sqlQuery);
            }
            else
            {
                // Create the OutsideActivity table if necessary
                _database.CreateTable<OutsideActivity>();
            }
        }

        public void UpdateOutsideActivity(OutsideActivity outsideActivity)
        {
            _database.Update(outsideActivity);
        }

        public void Dispose()
        {
            ((IDisposable)_database).Dispose();
        }

        public int CreateUser(User newUser, bool defaultUser = false)
        {
            // set defaultUser setting
            if(defaultUser)
            {
                string updateDefaultUserValueQuery = "Update User set DefaultUser = false";
                _database.ExecuteScalar<User>(updateDefaultUserValueQuery);
            }

            return _database.Insert(newUser);
        }
       
        public User GetDefaultUser()
        {
            User defaultUser = _database.Table<User>().Where(a => a.DefaultUser.Equals(1)).FirstOrDefault();

            if(defaultUser == null)
            {
                defaultUser = _database.Table<User>().FirstOrDefault();
                User newUser = new User();
                newUser.DefaultUser = true;

                _database.Insert(newUser);

                defaultUser = _database.Table<User>().Where(a => a.DefaultUser.Equals(1)).FirstOrDefault();
            }

            return defaultUser;
        }

        public List<User> GetAllUsers()
        {
            return _database.Table<User>().ToList();
        }
        
        public User GetUser(int id = 0)
        {
            return _database.Table<User>().Where(a => a.UserId.Equals(id)).FirstOrDefault();
        }

        public void deleteAllUsers()
        {
            _database.DeleteAll<User>();
        }
    }
}