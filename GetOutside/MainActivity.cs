using GetOutside.Database;
using GetOutside.Core.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;

namespace GetOutside
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        private Button _startActivityButton;
        private Button _resumeActivityButton;
        private Button _discardActivityButton;
        private Button _saveActivityButton;
        private Button _pauseActivityButton;
        private Button _addActivityButton;
        private Button _viewOutsideHoursButton;
        private Chronometer _currentActivityChronometer;
        private TextView _currentActivityTextView;
        public SqliteDataService _dataService = new SqliteDataService();
        //private Button _recordHoursButton;

        private outsideActivity _currentOutsideActivity = new outsideActivity();
        private object super;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);

            _dataService.Initialize();

            FindViews();
            LinkEventHandlers();
            SetSaveButtonView();
        }

        private void LinkEventHandlers()
        {
            _startActivityButton.Click += _startActivityButton_Click;
            _resumeActivityButton.Click += _resumeActivityButton_Click;
            _pauseActivityButton.Click += _pauseActivityButton_Click;
            _saveActivityButton.Click += _saveActivityButton_Click;
            _addActivityButton.Click += _addActivityButton_Click;
            _viewOutsideHoursButton.Click += _viewOutsideHoursButton_Click;
            _discardActivityButton.Click += _discardActivityButton_Click;
       }

        private void _discardActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime();
            _dataService.DeleteOutsideActivity(_currentOutsideActivity);

            SetDiscardButtonView();
        }

        private void _viewOutsideHoursButton_Click(object sender, EventArgs e)
        {
            //SetShowTotalHoursButtonView();
            try
            {
                TimeSpan outsideHours = _dataService.GetOutsideHours();
                if(outsideHours.TotalMilliseconds > 0)
                {
                    _currentActivityChronometer.Base = SystemClock.ElapsedRealtime() - (long)outsideHours.TotalMilliseconds; //SystemClock.ElapsedRealtime();

                    Intent intent = new Intent(this, typeof(outsideActivityAggregationActivity));
                    StartActivity(intent);
                    //Toast.MakeText(Application.Context, "Total outside hours: " + string.Format("{0:hh\\:mm\\:ss}", outsideHours), ToastLength.Short).Show();
                }
                else
                {
                    string toastMessage = "You can either create a new activity or enter previous activities";
                    Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
                }
            }
            catch
            {
                string toastMessage = "You can either create a new activity or enter previous activities";
                Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
            }
        }

        private void SetShowTotalHoursButtonView()
        {
            _currentActivityTextView.Text = "Total Outside Time";
        }

        private void SetStartButtonView()
        {
            // update textView for outside activity time
            _currentActivityTextView.Text = "Current Outside Time";

            // set button visibility
            _startActivityButton.Visibility = ViewStates.Invisible;
            _addActivityButton.Visibility = ViewStates.Invisible;
            _discardActivityButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Visible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _saveActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;

        }

        private void SetResumeButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Invisible;
            _addActivityButton.Visibility = ViewStates.Invisible;
            _discardActivityButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Visible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _saveActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;

        }
        
        private void SetPauseButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Invisible;
            _addActivityButton.Visibility = ViewStates.Invisible;
            _discardActivityButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Visible;
            _saveActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;
        }

        private void SetSaveButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Visible;
            _addActivityButton.Visibility = ViewStates.Visible;
            _discardActivityButton.Visibility = ViewStates.Invisible;
            _viewOutsideHoursButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _saveActivityButton.Visibility = ViewStates.Invisible;

        }

        private void SetDiscardButtonView()
        {
            _startActivityButton.Visibility = ViewStates.Visible;
            _addActivityButton.Visibility = ViewStates.Visible;
            _discardActivityButton.Visibility = ViewStates.Invisible;
            _viewOutsideHoursButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _saveActivityButton.Visibility = ViewStates.Invisible;

        }

        private void _pauseActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();
            _currentOutsideActivity.DurationMilliseconds = SystemClock.ElapsedRealtime() - _currentActivityChronometer.Base;
            SetPauseButtonView();
        }

        private void _saveActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();

            //Finalize OutsideActivityDatabase entry - set end time, durationMilliseconds, Name, done flag
            //_currentOutsideActivity.StartTime = new DateTime(2020, 3, 11, 20, 12, 13);
            //_currentOutsideActivity.EndTime = new DateTime(2020, 3, 11, 22, 12, 13);
            //_currentOutsideActivity.DurationMilliseconds = 7200000;
            //_currentOutsideActivity.YearMonth = "2020-03";

            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.Done = true;

            // write to the db
            // database.SaveItemAsync(_currentOutsideActivity);
            SetSaveButtonView();
            _dataService.UpdateOutsideActivity(_currentOutsideActivity);

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
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.YearMonth = _currentOutsideActivity.StartTime.ToString("yyyy'-'MM");

            _dataService.CreateOutsideActivity(_currentOutsideActivity);

            SetStartButtonView();
        }

        private void _resumeActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime() - _currentOutsideActivity.DurationMilliseconds;
            _currentActivityChronometer.Start();
            SetResumeButtonView();
        }

        private void _addActivityButton_Click(object sender, EventArgs e)
        {
            Intent manualOutsideEntryActivityIntent = new Intent(this, typeof(ManualOutsideEntryActivity));
            StartActivity(manualOutsideEntryActivityIntent);
        }

        private void FindViews()
        {
             _startActivityButton = FindViewById<Button>(Resource.Id.startActivityButton);
            _saveActivityButton = FindViewById<Button>(Resource.Id.saveActivityButton);
            _pauseActivityButton = FindViewById<Button>(Resource.Id.pauseActivityButton);
            _discardActivityButton = FindViewById<Button>(Resource.Id.discardActivityButton);
            _resumeActivityButton = FindViewById<Button>(Resource.Id.resumeActivityButton);
            _addActivityButton = FindViewById<Button>(Resource.Id.addActivityButton);
            _currentActivityChronometer = FindViewById<Chronometer>(Resource.Id.currentActivityChronometer);
            _viewOutsideHoursButton = FindViewById<Button>(Resource.Id.viewOutsideHoursButton);
            _currentActivityTextView = FindViewById<TextView>(Resource.Id.currentActivityTextView);

        }

        //protected override void OnSaveInstanceState(Bundle outState)
        //{
        //    //Finalize OutsideActivityDatabase entry - set end time, durationMilliseconds, Name, done flag
        //    _currentOutsideActivity.EndTime = DateTime.Now;
        //    _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
        //    _currentOutsideActivity.Done = false;

        //    //// write to the db
        //    _dataService.UpdateOutsideActivity(_currentOutsideActivity);

        //    base.OnSaveInstanceState(outState);
        //}

        //protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        //{
        //    base.OnRestoreInstanceState(savedInstanceState);
        //}

        protected void onDestroy()
        {
            //Finalize OutsideActivityDatabase entry - set end time, durationMilliseconds, Name, done flag
            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.Done = false;

            //// write to the db
            _dataService.UpdateOutsideActivity(_currentOutsideActivity);
        }
        protected void onStop()
        {
            // Finalize OutsideActivityDatabase entry -set end time, durationMilliseconds, Name, done flag
            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.Done = false;

            _dataService.UpdateOutsideActivity(_currentOutsideActivity);
        }

        protected void onStart()
        {
            // Finalize OutsideActivityDatabase entry -set end time, durationMilliseconds, Name, done flag
            _currentOutsideActivity = _dataService.GetLatestInProgressOutsideActivity();
            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString());
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _currentOutsideActivity.Done = false;
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

