using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Widget;
using GetOutside.Core.Model;
using SQLite;

namespace GetOutside.Database
{
    public class SqliteDataService : iLocalDataService, IDisposable
    {
        private SQLiteConnection _database;

        public void CreateOutsideActivity(outsideActivity outsideActivity)
        {
            _database.Insert(outsideActivity);
        }

        public void DeleteOutsideActivity(outsideActivity outsideActivity)
        {
            _database.Delete(outsideActivity);
        }

        public List<outsideActivity> GetOutsideActivity()
        {
            System.Collections.Generic.List<outsideActivity> outsideActivities = new System.Collections.Generic.List<outsideActivity>();
            //            return _database.Table<outsideActivity>().ToList();
            outsideActivities = _database.Query<outsideActivity>("select StartTime, DurationMilliseconds,name from outsideActivity order by StartTime desc");
            return outsideActivities;
        }

        public outsideActivity GetOutsideActivity(int id)
        {
            string getOutsideActivityQuery = String.Format("select OutsideActivityId, StartTime, DurationMilliseconds,name from outsideActivity where outsideActivityId = {0} limit 1", id);
            List<outsideActivity> outsideActivities = _database.Query<outsideActivity>(getOutsideActivityQuery);
            outsideActivity retreivedOutsideActivity = outsideActivities[0];

            return retreivedOutsideActivity;
        }

        public outsideActivity GetLatestOutsideActivity()
        {
            //            return _database.Table<outsideActivity>().ToList();
            string query = "select OutsideActivityId, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, isTracking from outsideActivity order by StartTime desc limit 1";
            System.Collections.Generic.List<outsideActivity> outsideActivities = _database.Query<outsideActivity>(query);

            return outsideActivities[0];
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideHoursByMonth()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds from outsideActivity group by strftime('%Y-%m',date(StartTime/10000000 - 62135596800, 'unixepoch'))  order by StartTime desc";
            System.Collections.Generic.List<outsideActivity> outsideHoursByMonth = _database.Query<outsideActivity>(query);
            return outsideHoursByMonth;
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideHoursByDay()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds from outsideActivity group by strftime('%Y-%m-%d',date(StartTime/10000000 - 62135596800, 'unixepoch')) order by StartTime desc";
            System.Collections.Generic.List<outsideActivity> outsideHoursByDay = _database.Query<outsideActivity>(query);

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
            catch
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
                //string dbPath = Path.Combine("/acct/", "GetOutsideDatabase.db3");
                _database = new SQLiteConnection(dbPath);
            }

            // Create the user table if necessary
            _database.CreateTable<User>();

            // check if outsideActivity table auto populates outsideActivityId column. If not, re-create table with auto populate outsideActivityId column
            string outsideActivityIdColumnIsNullQuery = "SELECT count(outsideActivityId) FROM outsideActivity where OutsideActivityId is not null";
            Int64 outsideActivityIdNullCount = _database.ExecuteScalar<Int64>(outsideActivityIdColumnIsNullQuery);

            if (outsideActivityIdNullCount == 0)
            {
                string sqlQuery = "BEGIN TRANSACTION; CREATE TEMPORARY TABLE OutsideActivity_backup(Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, isTracking); INSERT INTO OutsideActivity_backup SELECT (Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, isTracking) FROM OutsideActivity; DROP TABLE OutsideActivity; CREATE TABLE OutsideActivity(OutsideActivityId INTEGER PRIMARY KEY, Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, isTracking); INSERT INTO OutsideActivity SELECT (Name, StartTime, EndTime, DurationMilliseconds, ActivityType, UserId, Notes, Done, YearMonth, isTracking) FROM OutsideActivity_backup; DROP TABLE OutsideActivity_backup; COMMIT; ";
                _database.Query<outsideActivity>(sqlQuery);
            }
            //_database.CreateTable<outsideActivity>();

        }

        public void UpdateOutsideActivity(outsideActivity outsideActivity)
        {
            _database.Update(outsideActivity);
        }

        public void Dispose()
        {
            ((IDisposable)_database).Dispose();
        }

    }
}