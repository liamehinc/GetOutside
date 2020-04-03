using System;
using System.IO;
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
            _database.Query<outsideActivity>("update outsideActivity SET YearMonth = strftime('%Y-%m',StartTime)");
            if (outsideActivity != null)
            {
                outsideActivity.YearMonth = outsideActivity.StartTime.ToString("yyyy-MM");
            }
            _database.Insert(outsideActivity);
        }

        public void DeleteOutsideActivity(outsideActivity outsideActivity)
        {
            _database.Delete(outsideActivity);
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideActivity()
        {
            return _database.Table<outsideActivity>().ToList();
        }

        public System.Collections.Generic.List<outsideActivity> GetOutsideHoursByMonth()
        {
            System.Collections.Generic.List<outsideActivity> outsideHoursByMonth = _database.Query<outsideActivity>("SELECT * FROM outsideActivity order by StartTime");
            //var outsideHoursByMonth = _database.Query<outsideActivity>("SELECT strftime('%Y-%m', StartTime) as StartTime, SUM(DurationMilliseconds) FROM outsideActivity Group By strftime('%Y-%m',StartTime)");

            return outsideHoursByMonth;
        }

        public TimeSpan GetOutsideHours()
        {
            try
            {
                Int64 outsideTime = _database.ExecuteScalar<int>("Select SUM(DurationMilliseconds) FROM OutsideActivity where DurationMilliseconds > 0");
                DateTime dt = new DateTime(outsideTime);
                long ticks = dt.Ticks * 10000;

                return new TimeSpan(ticks);
            }
            catch
            {
                //Toast.MakeText(Application.Context, "There are not activities saved yet", ToastLength.Short).Show();
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

            _database.CreateTable<User>();
            _database.CreateTable<outsideActivity>();
        }

        public void UpdateOutsideActivity(outsideActivity outsideActivity)
        {
            if (outsideActivity.YearMonth == null)
            {
                outsideActivity.YearMonth = outsideActivity.StartTime.ToString("yyyy-MM");
            }
            _database.Update(outsideActivity);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}