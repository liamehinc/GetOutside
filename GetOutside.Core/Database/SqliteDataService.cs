using System;
using System.IO;
using GetOutside.Core.Model;
using SQLite;

namespace GetOutside.Database
{
    public class SqliteDataService : iLocalDataService
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

        public System.Collections.Generic.List<OutsideActivity> GetOutsideActivity()
        {
            return _database.Table<OutsideActivity>().ToList();
        }

        public TimeSpan GetOutsideHours()
        {
            Int64 outsideTime = _database.ExecuteScalar<int>("Select SUM(DurationMilliseconds) FROM OutsideActivity where DurationMilliseconds > 0");
            DateTime dt = new DateTime(outsideTime);
            long ticks = dt.Ticks * 10000;

            return new TimeSpan(ticks);
        }
        public void Initialize()
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "ThousandHoursOutsideDb.db3");
                _database = new SQLiteConnection(dbPath);
            }

            _database.CreateTable<User>();
            _database.CreateTable<OutsideActivity>();
        }

        public void UpdateOutsideActivity(OutsideActivity outsideActivity)
        {
            _database.Update(outsideActivity);
        }
    }
}