using System;
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

        public System.Collections.Generic.List<outsideActivity> GetOutsideActivity()
        {
            //            return _database.Table<outsideActivity>().ToList();
            System.Collections.Generic.List<outsideActivity> outsideActivities = _database.Query<outsideActivity>("select StartTime, DurationMilliseconds,name from outsideActivity order by StartTime desc");
            return outsideActivities;
        }

        public outsideActivity GetLatestInProgressOutsideActivity()
        {
            //            return _database.Table<outsideActivity>().ToList();
            string query = "select StartTime, DurationMilliseconds,Done,datetime('now') from outsideActivity where date(StartTime/ 10000000 - 62135596800,'unixepoch') > date('now', '-1 day') AND done = false order by StartTime desc limit 1";
            System.Collections.Generic.List<outsideActivity> outsideActivities = _database.Query<outsideActivity>(query);

            return outsideActivities[0];
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideHoursByMonth()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds  from outsideActivity group by strftime('%Y-%m',date(StartTime/10000000 - 62135596800, 'unixepoch')) order by StartTime desc";
            System.Collections.Generic.List<outsideActivity> outsideHoursByMonth = _database.Query<outsideActivity>(query);
            return outsideHoursByMonth;
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideHoursByDay()
        {
            string query = "select StartTime, sum(DurationMilliseconds) as DurationMilliseconds  from outsideActivity group by strftime('%Y-%m-%d',date(StartTime/10000000 - 62135596800, 'unixepoch')) order by StartTime desc";
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

            _database.CreateTable<User>();
            _database.CreateTable<outsideActivity>();
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