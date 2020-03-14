using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetOutside;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace GetOutside
{
    [Activity(Label = "ViewDashboardActivity", Theme = "@style/AppTheme")]
    class ViewDashboardActivity : MainActivity//, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private int _totalHours;
        private int _goalHours = 1000;
        private Button _addOutsideHoursButton;
        private Button _viewTotalOutsideHoursButton;
        private EditText _hoursToAddText;
        private Chronometer _viewTotalOutsideHourschronometer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.view_dashboard);

            FindViews();
            //   BindData();
            LinkEventHandlers();
        }

        private void LinkEventHandlers()
        {
            //_addOutsideHoursButton.Click += _addOutsideHoursButton_Click;
            _viewTotalOutsideHoursButton.Click += _viewTotalOutsideHoursButton_Click;
        }

        private void _viewTotalOutsideHoursButton_Click(object sender, EventArgs e)
        {
            TimeSpan outsideHours = _dataService.GetOutsideHours();
            _viewTotalOutsideHourschronometer.Base = SystemClock.ElapsedRealtime() - (long)outsideHours.TotalMilliseconds; //SystemClock.ElapsedRealtime();
            Toast.MakeText(Application.Context, "Total outside hours: " + string.Format("{0:hh\\:mm\\:ss}", outsideHours), ToastLength.Long).Show();
        }

        //private long convertChronometerToDuration(string chronoText)
        //{
        //    long durationMilliseconds = 0;
        //    String[] chronoArray = chronoText.Split(":");
        //    if (chronoArray.Length == 2)
        //    {
        //        durationMilliseconds = Convert.ToInt64(chronoArray[0]) * 60 * 1000
        //            + Convert.ToInt64(chronoArray[1]) * 1000;
        //    }
        //    else if (chronoArray.Length == 3)
        //    {
        //        durationMilliseconds = Convert.ToInt64(chronoArray[0]) * 60 * 60 * 1000
        //            + Convert.ToInt64(chronoArray[1]) * 60 * 1000
        //            + Convert.ToInt64(chronoArray[2]) * 1000;
        //    }

        //    return durationMilliseconds;
        //}
        private void _addOutsideHoursButton_Click(object sender, EventArgs e)
        {
            var hoursToAdd = int.Parse(_hoursToAddText.Text);
            _totalHours += hoursToAdd;
            //TotalHoursRepository totalHoursRepository = new TotalHoursRepository();
            //totalHoursRepository.AddToTotalHours(hoursToAdd);

            Toast.MakeText(Application.Context, "Hours total hours so far: " + _totalHours, ToastLength.Long).Show();
        }

        private void FindViews()
        {
            _viewTotalOutsideHourschronometer = FindViewById<Chronometer>(Resource.Id.viewTotalOutsideHoursChronometer);
            _viewTotalOutsideHoursButton = FindViewById<Button>(Resource.Id.viewTotalOutsideHoursButton);
            //_totalHours = 350;
            //_goalHourProgressBar.SetProgress(_totalHours, false);
        }
    }
}