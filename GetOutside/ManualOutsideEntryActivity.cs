using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GetOutside.Core.Model;
using GetOutside.Database;

namespace GetOutside
{
    [Activity(Label = "ManualOutsideEntryActivity")]
    public class ManualOutsideEntryActivity : Activity
    {
        public SqliteDataService _dataService = new SqliteDataService();
        public Button _addHoursButton;
        public EditText _dateOfActivityText;
        public EditText _hoursToAddText;

        public DatePicker _dateOfActivityDatePicker;
        public NumberPicker _hoursToAddNumberPicker;
        private outsideActivity _newOutsideActivity = new outsideActivity();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.manual_outside_activity_entry);

            _dataService.Initialize();

            FindViews();
            //BindData();
            LinkEventHandlers();

            _hoursToAddNumberPicker.MaxValue = 24;
            _dateOfActivityDatePicker.MaxDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            _dateOfActivityDatePicker.MinDate = DateTimeOffset.Now.AddYears(-20).ToUnixTimeMilliseconds();
            
        }

        private void BindData()
        {
            throw new NotImplementedException();
        }

        private void LinkEventHandlers()
        {
            _addHoursButton.Click += _addHoursButton_Click;
            //_datePicker.
        }

        private void _addHoursButton_Click(object sender, EventArgs e)
        {
            // Get the base starting time and start the timer
            _newOutsideActivity.Done = true;
            //_newOutsideActivity.StartTime = DateTime.Parse(_dateOfActivityText.Text);
            //_newOutsideActivity.DurationMilliseconds = int.Parse(_hoursToAddText.Text) * 3600000;
            
            _newOutsideActivity.StartTime = _dateOfActivityDatePicker.DateTime;
            _newOutsideActivity.DurationMilliseconds = _hoursToAddNumberPicker.Value * 3600000;
            _newOutsideActivity.YearMonth = _newOutsideActivity.StartTime.ToString("yyyy'-'MM");
            _newOutsideActivity.Name = "outsideActivity-" + _newOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            
            //_newOutsideActivity.EndTime = _newOutsideActivity.StartTime.AddMilliseconds(_newOutsideActivity.DurationMilliseconds);

            _dataService.CreateOutsideActivity(_newOutsideActivity);
            base.OnBackPressed();
        }

        private void FindViews()
        {
            _addHoursButton = FindViewById<Button>(Resource.Id.addHoursButton);
            _hoursToAddNumberPicker = FindViewById<NumberPicker>(Resource.Id.hoursToAddNumberPicker);
            _dateOfActivityDatePicker = FindViewById<DatePicker>(Resource.Id.dateOfActivityDatePicker);

        }
    }
}