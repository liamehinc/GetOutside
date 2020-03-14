using Android.App;
using GetOutside.Core.Model;
using Android.OS;
using System;
using Android.Widget;
using GetOutside;

namespace GetOutside
{
    [Activity(Label = "RecordHoursActivity")]
    public class RecordHoursActivity : Activity
    {
        private int _totalHours;
        private Button _addHoursButton;
        private EditText _hoursToAddText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.record_hours);

            FindViews();
         //   BindData();
            LinkEventHandlers();
        }

        private void BindData()
        {
           // _totalHours = _totalHoursRepository.TimeOutside;
        }

        private void FindViews()
        {
            _hoursToAddText = FindViewById<EditText>(Resource.Id.hoursToAddText);
            _addHoursButton = FindViewById<Button>(Resource.Id.addHoursButton);
        }

        private void LinkEventHandlers()
        {
            _addHoursButton.Click += _addHoursButton_Click;
        }

        private void _addHoursButton_Click(object sender, EventArgs e)
        {
            var hoursToAdd = int.Parse(_hoursToAddText.Text);
            _totalHours += hoursToAdd;
            //TotalHoursRepository totalHoursRepository = new TotalHoursRepository();
            //totalHoursRepository.AddToTotalHours(hoursToAdd);

            Toast.MakeText(Application.Context, "Hours total hours so far: " + _totalHours, ToastLength.Long).Show();

        }
    }
}