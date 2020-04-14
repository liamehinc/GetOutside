using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GetOutside.Core.Model;
using GetOutside.Database;
using static Android.App.DatePickerDialog;
using static Android.App.TimePickerDialog;

namespace GetOutside
{
    [Activity(Label = "EditOutsideActivity", Theme = "@style/AppTheme")]

    public class EditOutsideActivityActivity : Activity, IOnDateSetListener, IOnTimeSetListener
    {
        private EditText _editOutsideActivityNameEditText;
        private EditText _editOutsideActivityStartDateEditText;
        private EditText _editOutsideActivityStartTimeEditText;
        private TimePicker _editOutsideActivityDurationTimePicker;
        TimeSpan currentDuration;
        TimeSpan newDuration;

        private Button _updateOutsideActivityButton;

        private outsideActivity outsideActivity;

        private SqliteDataService _dataService = new SqliteDataService();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.edit_outside_activity);

            var selectedOutsideActivityId = Intent.Extras.GetInt("selectedOutsideActivityId");

            _dataService.Initialize();
            outsideActivity = _dataService.GetOutsideActivity(selectedOutsideActivityId);
            //outsideActivity = new outsideActivity();
            
            //outsideActivity.Name = "outsideActivity-2020032815193912";
            //outsideActivity.StartTime = new DateTime(2020, 3, 11, 08, 12, 13);
            //outsideActivity.EndTime = new DateTime(2020, 3, 11, 10, 12, 13);
            //outsideActivity.DurationMilliseconds = 7200000;
            //currentDuration = TimeSpan.FromMilliseconds(outsideActivity.DurationMilliseconds);
            //outsideActivity = outsideActivities[0];

            FindViews();
            LinkEventHandlers(); 
        }

        private void LinkEventHandlers()
        {
            _editOutsideActivityNameEditText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
                outsideActivity.Name = e.Text.ToString();
            };

            _editOutsideActivityStartDateEditText.Click += _editOutsideActivityStartDateEditText_Click; ;
            _editOutsideActivityStartTimeEditText.Click += _editOutsideActivityStartTimeEditText_Click; ;
            _updateOutsideActivityButton.Click += _updateOutsideActivityButton_Click;

        }

        private void _editOutsideActivityDurationEditText_Click(object sender, EventArgs e)
        {
            bool is24HourFormat = true;
            TimePickerDialog timePicker = new TimePickerDialog(this, TimePickerDialog.ThemeHoloLight, this, currentDuration.Hours, currentDuration.Minutes, is24HourFormat);
            timePicker.Show();
        }

        private void _editOutsideActivityStartTimeEditText_Click(object sender, EventArgs e)
        {
            bool is24HourFormat = false;
            TimePickerDialog timePicker = new TimePickerDialog(this, TimePickerDialog.ThemeHoloLight, this, outsideActivity.StartTime.Hour, outsideActivity.StartTime.Minute, is24HourFormat);
            timePicker.Show();
        }

        private void _editOutsideActivityStartDateEditText_Click(object sender, EventArgs e)
        {
            DatePickerDialog datePicker = new DatePickerDialog(this, this, outsideActivity.StartTime.Year, outsideActivity.StartTime.Month - 1, outsideActivity.StartTime.Day);
            datePicker.Show();
        }

        private void _updateOutsideActivityButton_Click(object sender, EventArgs e)
        {
            // set isTracking and Done
            outsideActivity.isTracking = false;
            outsideActivity.Done = true;

            // determine the new duration of the activity
            newDuration = new TimeSpan(_editOutsideActivityDurationTimePicker.Hour, _editOutsideActivityDurationTimePicker.Minute, 0);

            // Set new start and end times
            if (DateTime.TryParse(_editOutsideActivityStartDateEditText.Text, out DateTime newStartDate))
            {
                string newStartDateTime = newStartDate.ToShortDateString() + " " + _editOutsideActivityStartTimeEditText.Text;
                outsideActivity.StartTime = Convert.ToDateTime(newStartDateTime, CultureInfo.CurrentCulture);
                outsideActivity.EndTime = outsideActivity.StartTime.Add(newDuration);
            }

            // set durationMilliseconds
            outsideActivity.DurationMilliseconds = (long) newDuration.TotalMilliseconds;

            // update the outdoor activity
            //_dataService.UpdateOutsideActivity(outsideActivity);
        }

        private void FindViews()
        {
            // set up data entry fields
            _editOutsideActivityNameEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityNameEditText);
            _editOutsideActivityStartDateEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityStartDateEditText);
            _editOutsideActivityStartTimeEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityStartTimeEditText);
            _editOutsideActivityDurationTimePicker = FindViewById<TimePicker>(Resource.Id.editOutsideActivityDurationTimePicker);
            _updateOutsideActivityButton = FindViewById<Button>(Resource.Id.updateOutsideActivityButton);            
            
            // set default values for data entry fields based on outdoorAcivity
            _editOutsideActivityNameEditText.SetText(outsideActivity.Name, TextView.BufferType.Editable);

            // convert duration Milliseconds to hours and minutes
            _editOutsideActivityDurationTimePicker.Hour = currentDuration.Hours; //outsideActivity.DurationMilliseconds;
            _editOutsideActivityDurationTimePicker.Minute = currentDuration.Minutes; //currentTime.Minute;

            // set up TimePicker for outsideActivityDuration
            _editOutsideActivityDurationTimePicker.SetIs24HourView((Java.Lang.Boolean)true);

            // set default start date
            _editOutsideActivityStartDateEditText.SetText(outsideActivity.StartTime.ToShortDateString(), TextView.BufferType.Editable);

            //set default start time
            _editOutsideActivityStartTimeEditText.SetText(outsideActivity.StartTime.ToShortTimeString(), TextView.BufferType.Editable);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            FindViewById<EditText>(Resource.Id.editOutsideActivityStartDateEditText).Text = new DateTime(year, month + 1, dayOfMonth, outsideActivity.StartTime.Hour, outsideActivity.StartTime.Minute, outsideActivity.StartTime.Second).ToShortDateString();
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime selectedTime = new DateTime(outsideActivity.StartTime.Year, outsideActivity.StartTime.Month, outsideActivity.StartTime.Day, hourOfDay, minute, outsideActivity.StartTime.Second);
            FindViewById<EditText>(Resource.Id.editOutsideActivityStartTimeEditText).Text = selectedTime.ToShortTimeString();
        }
    }

  }