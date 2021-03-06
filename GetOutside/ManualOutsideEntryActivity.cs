﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
    [Activity(Label = "ManualOutsideEntryActivity", Theme = "@style/AppTheme")]
    public class ManualOutsideEntryActivity : Activity
    {
        private SqliteDataService _dataService = new SqliteDataService();
        private Button _addHoursButton;
        //private EditText _dateOfActivityText;
        //private EditText _hoursToAddText;

        private DatePicker _dateOfActivityDatePicker;
        private NumberPicker _hoursToAddNumberPicker;
        private OutsideActivity _newOutsideActivity = new OutsideActivity();

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
            _newOutsideActivity.IsTracking = false;
            
            _newOutsideActivity.StartTime = _dateOfActivityDatePicker.DateTime;
            _newOutsideActivity.DurationMilliseconds = _hoursToAddNumberPicker.Value * 3600000;
            _newOutsideActivity.Name = "outsideActivity-" + _newOutsideActivity.StartTime.ToString("yyyyMMddHHmmssff", CultureInfo.CurrentCulture);
            
            _newOutsideActivity.EndTime = _newOutsideActivity.StartTime.AddMilliseconds(_newOutsideActivity.DurationMilliseconds);

            _dataService.CreateOutsideActivity(_newOutsideActivity);
            //base.OnBackPressed();
            string toastMessage = String.Format(CultureInfo.CurrentCulture, "Inserted {0} activity", _newOutsideActivity.Name);
            Toast.MakeText(Application.Context, toastMessage, ToastLength.Short).Show();
            Finish();
        }

        private void FindViews()
        {
            _addHoursButton = FindViewById<Button>(Resource.Id.addHoursButton);
            _hoursToAddNumberPicker = FindViewById<NumberPicker>(Resource.Id.hoursToAddNumberPicker);
            _dateOfActivityDatePicker = FindViewById<DatePicker>(Resource.Id.dateOfActivityDatePicker);

        }
    }
}