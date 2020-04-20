﻿using GetOutside.Database;
using GetOutside.Core.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Globalization;

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
        private SqliteDataService _dataService = new SqliteDataService();

        // STATICS
        private static string TOTALOUTSIDETIME = "Total Outside Time";
        private static string CURRENTOUTSIDETIME = "Current Outside Time";
        private static string NEWACTIVITYORPREVIOUSACTIVITY = "You can either create a new activity or enter previous activities";

        private OutsideActivity _currentOutsideActivity = new OutsideActivity();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _dataService.Initialize();

            FindViews();
            LinkEventHandlers();
            SetSaveActivityView();
        }

        protected override void OnStart()
        {
            base.OnStart();

            // if there isn't a current activity that's not done, check if there is an incomplete activity in the DB
            if (_currentOutsideActivity.OutsideActivityId == 0)
            {
                _currentOutsideActivity = _dataService.GetLatestOutsideActivity();
            }
            
            // reset the chronometer and views
            if (_currentOutsideActivity.IsTracking || !(_currentOutsideActivity.Done))
            {
                //TimeSpan durationMillisecondsTimeSpan = new TimeSpan(_currentOutsideActivity.DurationMilliseconds);
                long newChronometerBaseOffset;

                // Reset the chronometer to the correct duration value accounting for time running in the background
                if (_currentOutsideActivity.IsPaused)
                {
                    // timer was paused in the background
                    newChronometerBaseOffset = (long)TimeSpan.FromMilliseconds(_currentOutsideActivity.DurationMilliseconds).TotalMilliseconds;
                    SetPauseActivityView();
                }
                else
                {
                    // timer was running in the background
                    TimeSpan currentActivityTimeSpan = _currentOutsideActivity.EndTime.Subtract(_currentOutsideActivity.StartTime);
                    TimeSpan timeSinceEndTimeRecorded = DateTime.Now - _currentOutsideActivity.EndTime;
                    newChronometerBaseOffset = (long)currentActivityTimeSpan.Add(timeSinceEndTimeRecorded).TotalMilliseconds;
                    SetActivityTimingView();
                }
                
                // Reset the Chronometer to account for a running activity
                ResetChronometer(newChronometerBaseOffset, _currentOutsideActivity.IsPaused);
            }
            else
            {
                ResetChronometer();
            }
        }

        protected override void OnStop()
        {
            // only need to update the activity in DB if it isn't paused
            if (_currentOutsideActivity.IsTracking && !_currentOutsideActivity.IsPaused)
            {
                // Finalize OutsideActivityDatabase entry -set end time, durationMilliseconds
                _updateCurrentActivity();
            }
            base.OnStop();
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
            // reset the chronometer
            _currentActivityChronometer.Stop();
            _currentOutsideActivity.IsPaused = true;
            _currentOutsideActivity.IsTracking = false;
            ResetChronometer();

            // delete the outside activity from the database and reset the outside activity
            _dataService.DeleteOutsideActivity(_currentOutsideActivity);
            _currentOutsideActivity = new OutsideActivity();

            // reset the view
            SetSaveActivityView();
        }

        private void _viewOutsideHoursButton_Click(object sender, EventArgs e)
        {
            //SetShowTotalHoursButtonView();
            try
            {
                TimeSpan outsideHours = _dataService.GetOutsideHours();
                if(outsideHours.TotalMilliseconds > 0)
                {
                    using (Intent intent = new Intent(this, typeof(outsideActivityAggregationActivity)))
                    {
                        StartActivity(intent);
                    }
                }
                else
                {
                    string toastMessage = NEWACTIVITYORPREVIOUSACTIVITY;
                    Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
                }
            }
            catch
            {
                string toastMessage = NEWACTIVITYORPREVIOUSACTIVITY;
                Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
            }
        }

        private void SetShowTotalHoursButtonView()
        {
            _currentActivityTextView.Text = TOTALOUTSIDETIME;
        }

        private void SetActivityTimingView()
        {
            // update textView for outside activity time
            _currentActivityTextView.Text = CURRENTOUTSIDETIME;

            // set button visibility
            _startActivityButton.Visibility = ViewStates.Invisible;
            _addActivityButton.Visibility = ViewStates.Invisible;
            _discardActivityButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Visible;
            _resumeActivityButton.Visibility = ViewStates.Invisible;
            _saveActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;
        }
      
        private void SetPauseActivityView()
        {
            _startActivityButton.Visibility = ViewStates.Invisible;
            _addActivityButton.Visibility = ViewStates.Invisible;
            _discardActivityButton.Visibility = ViewStates.Visible;
            _pauseActivityButton.Visibility = ViewStates.Invisible;
            _resumeActivityButton.Visibility = ViewStates.Visible;
            _saveActivityButton.Visibility = ViewStates.Visible;
            _viewOutsideHoursButton.Visibility = ViewStates.Invisible;
        }

        private void SetSaveActivityView()
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
            _currentOutsideActivity.IsPaused = true;
            _updateCurrentActivity();
            SetPauseActivityView();
        }

        private void _updateCurrentActivity()
        {
            //Finalize OutsideActivityDatabase entry - set end time, durationMilliseconds, Name, done flag
            //_currentOutsideActivity.StartTime = new DateTime(2020, 3, 11, 20, 12, 13);
            //_currentOutsideActivity.EndTime = new DateTime(2020, 3, 11, 22, 12, 13);
            //_currentOutsideActivity.DurationMilliseconds = 7200000;

            _currentOutsideActivity.EndTime = DateTime.Now;
            _currentOutsideActivity.DurationMilliseconds = convertChronometerToDuration(_currentActivityChronometer.Text.ToString(CultureInfo.CurrentCulture));

            // Convert the duration of the activity to a TimeSpan
            if (!TimeSpan.TryParse(_currentActivityChronometer.Text.ToString(CultureInfo.CurrentCulture), out TimeSpan durationMillisecondsTimeSpan))
            {
                // handle the validation error
            }
            else
            {
                durationMillisecondsTimeSpan = TimeSpan.Parse(_currentActivityChronometer.Text.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
            }

            // write to the db
            _dataService.UpdateOutsideActivity(_currentOutsideActivity);
        }

        private void _saveActivityButton_Click(object sender, EventArgs e)
        {
            _currentActivityChronometer.Stop();

            _currentOutsideActivity.Done = true;
            _currentOutsideActivity.IsTracking = false;
            _currentOutsideActivity.IsPaused = false;

            // write to the db
            _updateCurrentActivity();
            SetSaveActivityView();

            // toast to notify activity was saved.
            DateTime dt = new DateTime(_currentOutsideActivity.DurationMilliseconds);
            long ticks = dt.Ticks * 10000;
            TimeSpan elapsedSpan = new TimeSpan(ticks);
            Toast.MakeText(Application.Context, "Current outside activity time: " + string.Format(CultureInfo.CurrentCulture, "{0:hh\\:mm\\:ss}", elapsedSpan), ToastLength.Short).Show();
        }

        private long convertChronometerToDuration(string chronoText)
        {
            long durationMilliseconds = 0;
            // revisit this
            //TimeSpan tempTimeSpan = TimeSpan.FromMilliseconds(Convert.ToDateTime(chronoText, CultureInfo.CurrentCulture).Millisecond);

            String[] chronoArray = chronoText.Split(":");
            if (chronoArray.Length == 2)
            {
                durationMilliseconds = Convert.ToInt64(chronoArray[0], CultureInfo.CurrentCulture) * 60 * 1000 
                    + Convert.ToInt64(chronoArray[1], CultureInfo.CurrentCulture) * 1000;
            }
            else if (chronoArray.Length == 3)
            {
                durationMilliseconds = Convert.ToInt64(chronoArray[0], CultureInfo.CurrentCulture) * 60 * 60 * 1000 // hours
                    + Convert.ToInt64(chronoArray[1], CultureInfo.CurrentCulture) * 60 * 1000 // minutes
                    + Convert.ToInt64(chronoArray[2], CultureInfo.CurrentCulture) * 1000; // seconds
            }

            return durationMilliseconds;
        }

        private void _startActivityButton_Click(object sender, EventArgs e)
        {
            // Get the base starting time and start the timer
            _currentOutsideActivity = new OutsideActivity(true);
            ResetChronometer();
            _currentOutsideActivity.StartTime = DateTime.Now;
            _currentOutsideActivity.Name = "outsideActivity-" + _currentOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff", CultureInfo.CurrentCulture);

            _dataService.CreateOutsideActivity(_currentOutsideActivity);

            SetActivityTimingView();
        }

        private void _resumeActivityButton_Click(object sender, EventArgs e)
        {
            _currentOutsideActivity.IsTracking = true;
            _currentOutsideActivity.IsPaused = false;
            ResetChronometer(_currentOutsideActivity.DurationMilliseconds, _currentOutsideActivity.IsPaused);

            SetActivityTimingView();
        }

        private void _addActivityButton_Click(object sender, EventArgs e)
        {
            using (Intent editOutsideActivityIntent = new Intent(this, typeof(EditOutsideActivityActivity)))
            {
                StartActivity(editOutsideActivityIntent);
            }
        }

        private void ResetChronometer(long currentDuration = 0, bool isPaused = false)
        {
            _currentActivityChronometer.Base = SystemClock.ElapsedRealtime() - currentDuration;
            if (_currentOutsideActivity.IsTracking && !_currentOutsideActivity.IsPaused)
            {
                _currentActivityChronometer.Start();
            }
        }
        private void FindViews()
        {
            textMessage = FindViewById<TextView>(Resource.Id.message);
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

