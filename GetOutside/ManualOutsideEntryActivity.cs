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
        }

        private void BindData()
        {
            throw new NotImplementedException();
        }

        private void LinkEventHandlers()
        {
            _addHoursButton.Click += _addHoursButton_Click; ;
        }

        private void _addHoursButton_Click(object sender, EventArgs e)
        {
            // Get the base starting time and start the timer
            _newOutsideActivity.Done = true;
            _newOutsideActivity.StartTime = DateTime.Parse(_dateOfActivityText.Text);
            _newOutsideActivity.DurationMilliseconds = int.Parse(_hoursToAddText.Text) * 3600000;
            _newOutsideActivity.YearMonth = _newOutsideActivity.StartTime.ToString("yyyy'-'MM");
            _newOutsideActivity.Name = "outsideActivity-" + _newOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff");
            _newOutsideActivity.EndTime = _newOutsideActivity.StartTime.AddMilliseconds(_newOutsideActivity.DurationMilliseconds);
            
            //_currentActivityChronometer.Base = SystemClock.ElapsedRealtime();
            //_currentActivityChronometer.Start();
            //_currentOutsideActivity.StartTime = DateTime.Now;
            //_currentOutsideActivity.YearMonth = _currentOutsideActivity.StartTime.ToString("yyyy'-'MM");

            _dataService.CreateOutsideActivity(_newOutsideActivity);
            base.OnBackPressed();
        }

        private void FindViews()
        {
            _addHoursButton = FindViewById<Button>(Resource.Id.addHoursButton);
            _dateOfActivityText = FindViewById<EditText>(Resource.Id.dateOfActivityText);
            _hoursToAddText = FindViewById<EditText>(Resource.Id.hoursToAddText);
//                FindViewById<EditText>(Resource.Id.hoursToAddText);
//            _dateOfActivityText = DateTime.Parse(.ToString());
            //EditText hoursToAdd = FindViewById<EditText>(Resource.Id.hoursToAddText);
            //_hoursToAdd = int.Parse(hoursToAdd);
            //_dateOfActivityText = DateTime.Parse(FindViewById<EditText>(Resource.Id.dateOfActivityText).ToString());
            //_hoursToAdd = FindViewById<EditText>(Resource.Id.hoursToAddText);
        }
    }
}