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
        private Button _deleteOutsideActivityButton;

        private List<OutsideActivity> outsideActivities;
        private OutsideActivity outsideActivity;

        private SqliteDataService _dataService = new SqliteDataService();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.edit_outside_activity);

            _dataService.Initialize();

            try
            {
                var selectedOutsideActivityId = Intent.Extras.GetInt("selectedOutsideActivityId");

                outsideActivities = _dataService.GetOutsideActivity();
                outsideActivity = outsideActivities[selectedOutsideActivityId];
                currentDuration = TimeSpan.FromMilliseconds(outsideActivity.DurationMilliseconds);
            }
            catch (System.NullReferenceException)
            {
                outsideActivity = new OutsideActivity();
                outsideActivity.StartTime = DateTime.Now.AddHours(-2);
                _dataService.CreateOutsideActivity(outsideActivity);
            }

            FindViews();
            LinkEventHandlers(); 
        }

        private void LinkEventHandlers()
        {
            _editOutsideActivityNameEditText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => {
                outsideActivity.Name = e.Text.ToString();
            };

            _editOutsideActivityStartDateEditText.Click += _editOutsideActivityStartDateEditText_Click;
            _editOutsideActivityStartTimeEditText.Click += _editOutsideActivityStartTimeEditText_Click;
            _updateOutsideActivityButton.Click += _updateOutsideActivityButton_Click;
            // popup dialog to confirm activity should be deleted
            _deleteOutsideActivityButton.Click += delegate {
                Android.App.AlertDialog.Builder alertDiag = new Android.App.AlertDialog.Builder(this);
                alertDiag.SetTitle("Confirm delete");
                alertDiag.SetMessage("Once deleted the activity cannot be recovered");
                alertDiag.SetPositiveButton("Delete", (senderAlert, args) => {
                    _dataService.DeleteOutsideActivity(outsideActivity);
                    
                    Toast.MakeText(this, "Deleted", ToastLength.Short).Show();
                    Finish();
                });
                alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            };

        }

        private void _editOutsideActivityStartDateEditText_Click(object sender, EventArgs e)
        {
            using (DatePickerDialog datePicker = new DatePickerDialog(this, this, outsideActivity.StartTime.Year, outsideActivity.StartTime.Month - 1, outsideActivity.StartTime.Day))
            {
                datePicker.Show();
            }
        }

        private void _editOutsideActivityDurationEditText_Click(object sender, EventArgs e)
        {
            bool is24HourFormat = true;
            using (TimePickerDialog timePicker = new TimePickerDialog(this, TimePickerDialog.ThemeHoloLight, this, currentDuration.Hours, currentDuration.Minutes, is24HourFormat))
            {
                timePicker.Show();
            }
        }

        private void _editOutsideActivityStartTimeEditText_Click(object sender, EventArgs e)
        {
            bool is24HourFormat = false;
            using (TimePickerDialog timePicker = new TimePickerDialog(this, TimePickerDialog.ThemeHoloLight, this, outsideActivity.StartTime.Hour, outsideActivity.StartTime.Minute, is24HourFormat))
            {
                timePicker.Show();
            }
        }

        private void _updateOutsideActivityButton_Click(object sender, EventArgs e)
        {
            string toastMessage;

            // set isTracking and Done
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

            // update or add the outdoor activity
            if (outsideActivity.OutsideActivityId == 0)
            {
                _dataService.CreateOutsideActivity(outsideActivity);
                toastMessage = String.Format("Inserted {0} activity", outsideActivity.Name);
            }
            else
            {
                _dataService.UpdateOutsideActivity(outsideActivity);
                toastMessage = String.Format("Updated {0} activity", outsideActivity.Name); 
            }

            // close this intent
            Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
            Finish();
        }

        private void FindViews()
        {
            // set up data entry fields
            _editOutsideActivityNameEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityNameEditText);
            _editOutsideActivityStartDateEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityStartDateEditText);
            _editOutsideActivityStartTimeEditText = FindViewById<EditText>(Resource.Id.editOutsideActivityStartTimeEditText);
            _editOutsideActivityDurationTimePicker = FindViewById<TimePicker>(Resource.Id.editOutsideActivityDurationTimePicker);
            _updateOutsideActivityButton = FindViewById<Button>(Resource.Id.updateOutsideActivityButton);
            _deleteOutsideActivityButton = FindViewById<Button>(Resource.Id.deleteOutsideActivityButton);
            
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