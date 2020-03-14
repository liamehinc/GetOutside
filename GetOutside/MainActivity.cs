using GetOutside.Database;
using GetOutside.Core.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using GetOutside;

namespace GetOutside
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        private Button _startActivityButton;
        private Button _resumeActivityButton;
        private Button _stopActivityButton;
        private Button _pauseActivityButton;
        private Button _viewOutsideHoursButton;
        private Chronometer _currentActivityChronometer;
        private TextView _currentActivityTextView;
        public SqliteDataService _dataService = new SqliteDataService();
        //private Button _recordHoursButton;
        //private TimePicker _previousOutsideHoursTimePicker;

        private OutsideActivity _currentOutsideActivity = new OutsideActivity();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            //BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            //navigation.SetOnNavigationItemSelectedListener(this);

            _dataService.Initialize();

            FindViews();
            LinkEventHandlers();
        }

        private void LinkEventHandlers()
        {
            //_recordHoursButton.Click += _recordHoursButton_Click;
            _startActivityButton.Click += _startActivityButton_Click;
            _resumeActivityButton.Click += _resumeActivityButton_Click;
            _pauseActivityButton.Click += _pauseActivityButton_Click;
            _stopActivityButton.Click += _stopActivityButton_Click;
            _viewOutsideHoursButton.Click += _viewOutsideHoursButton_Click;
        }

        private void _viewOutsideHoursButton_Click(object sender, EventArgs e)
        {
            SetShowTotalHoursButtonView();

            TimeSpan outsideHours = _dataService.GetOutsideHours();
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime() - (long)outsideHours.TotalMilliseconds; //SystemClock.ElapsedRealtime();
           //Toast.MakeText(Application.Context, "Total outside hours: " + string.Format("{0:hh\\:mm\\:ss}", outsideHours), ToastLength.Short).Show();
        }

        private void SetShowTotalHoursButtonView()
        {
            _currentActivityTextView.Text = "Total Outside Time";
        }

        private void SetStartButtonView()
        {
            // update textView for outdoor activity time
            _currentActivityTextView.Text = "Current Outside Time";

            // set button visibility
            _startActivityButton.Visibility = ViewStates.Invisible;
            _pauseActivityButton.Visibility = ViewStates.Visible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _stopActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;

        }

        private void SetResumeButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Invisible;
            _pauseActivityButton.Visibility = ViewStates.Visible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _stopActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;

        }
        
        private void SetPauseButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Invisible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Visible;
            _stopActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;
        }

        private void SetStopButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _stopActivityButton.Visibility = ViewStates.Invisible;

            // Leave these invisible until they are implemented.
            //_previousOutsideHoursTimePicker.Visibility = ViewStates.Invisible;
            //_recordHoursButton.Visibility = ViewStates.Invisible;
        }

        private void _pauseActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();
            _currentOutsideActivity.DurationMilliseconds = SystemClock.ElapsedRealtime() - _currentActivityChronometer.Base;
            SetPauseButtonView();
        }

        private void _stopActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();

            // Finalize OutsideActivityDatabase entry - set end time, durationMilliseconds, Name, done flag
            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.EndTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.Done = true;

            // write to the db
            // database.SaveItemAsync(_currentOutsideActivity);
            SetStopButtonView();
            _dataService.CreateOutsideActivity(_currentOutsideActivity);

            DateTime dt = new DateTime(_currentOutsideActivity.DurationMilliseconds);
            long ticks = dt.Ticks * 10000;
            TimeSpan elapsedSpan = new TimeSpan(ticks);
            Toast.MakeText(Application.Context, "Current outside activity time: " + string.Format("{0:hh\\:mm\\:ss}", elapsedSpan), ToastLength.Short).Show();

        }

        private long convertChronometerToDuration(string chronoText)
        {
            long durationMilliseconds = 0;
            String[] chronoArray = chronoText.Split(":");
            if (chronoArray.Length == 2)
            {
                durationMilliseconds = Convert.ToInt64(chronoArray[0]) * 60 * 1000 
                    + Convert.ToInt64(chronoArray[1]) * 1000;
            }
            else if (chronoArray.Length == 3)
            {
                durationMilliseconds = Convert.ToInt64(chronoArray[0]) * 60 * 60 * 1000
                    + Convert.ToInt64(chronoArray[1]) * 60 * 1000
                    + Convert.ToInt64(chronoArray[2]) * 1000;
            }

            return durationMilliseconds;
        }

        private void _startActivityButton_Click(object sender, EventArgs e)
        {
            // Get the base starting time and start the timer
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime();
            _currentActivityChronometer.Start();
            _currentOutsideActivity.StartTime = DateTime.Now;

 
            SetStartButtonView();

        }

        private void _resumeActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime() - _currentOutsideActivity.DurationMilliseconds;
            _currentActivityChronometer.Start();
            SetResumeButtonView();

        }

        private void _recordHoursButton_Click(object sender, EventArgs e)
        {
            //_previousOutsideHoursTimePicker.Visibility = ViewStates.Visible; 
            //Intent intent = new Intent(this, typeof(RecordHoursActivity));
            //StartActivity(intent);
        }

        private void FindViews()
        {
            // _recordHoursButton = FindViewById<Button>(Resource.Id.recordHoursButton);
            //  _previousOutsideHoursTimePicker = FindViewById<TimePicker>(Resource.Id.previousOutsideHoursTimePicker);
            _startActivityButton = FindViewById<Button>(Resource.Id.startActivityButton);
            _stopActivityButton = FindViewById<Button>(Resource.Id.stopActivityButton);
            _pauseActivityButton = FindViewById<Button>(Resource.Id.pauseActivityButton);
            _resumeActivityButton = FindViewById<Button>(Resource.Id.resumeActivityButton);
            _currentActivityChronometer = FindViewById<Chronometer>(Resource.Id.currentActivityChronometer);
            _viewOutsideHoursButton = FindViewById<Button>(Resource.Id.viewOutsideHoursButton);
            _currentActivityTextView = FindViewById<TextView>(Resource.Id.currentActivityTextView);
            SetStopButtonView();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        //Bottom navigation item selected
        //public bool OnNavigationItemSelected(IMenuItem item)
        //{
        //    switch (item.ItemId)
        //    {
        //        case Resource.Id.navigation_home:
        //            Intent mainActivityIntent = new Intent(this, typeof(MainActivity));
        //            StartActivity(mainActivityIntent);
        //            return true;
        //        case Resource.Id.navigation_dashboard:
        //            Intent viewDashboardIntent = new Intent(this, typeof(ViewDashboardActivity));
        //            StartActivity(viewDashboardIntent);
        //            return true;
        //        case Resource.Id.navigation_notifications:
        //            textMessage.SetText(Resource.String.title_notifications);
        //            return true;
        //    }
        //    return false;
        //}
    }
}

