using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GetOutside.Core.Model;
using GetOutside.Database;
using GetOutside.ViewHolders;

namespace GetOutside.Adapters
{

    class outsideActivityAdapter : RecyclerView.Adapter
    {
        private List<outsideActivity> _outsideActivitiesByMonth;
        public SqliteDataService _dataService = new SqliteDataService();

        public outsideActivityAdapter()
        {
            _dataService.Initialize();
            _outsideActivitiesByMonth = _dataService.GetOutsideHoursByMonth();

            var query = _outsideActivitiesByMonth.GroupBy(
                            outsideActivity => outsideActivity.StartTime.ToString("yyyy'-'MM"),
                            outsideActivity => outsideActivity.DurationMilliseconds,
                            (yearMonth, durations) => new
                            {
                                Key = yearMonth,
                                timeOutside = durations.Sum() / 36000
                            });

        }
        public override int ItemCount => _outsideActivitiesByMonth.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is OutsideActivityViewHolder outsideActivityViewHolder)
            {
                // convert the durationMilliseconds to dateTime format
                //TimeSpan duration = TimeSpan.FromMilliseconds(_outsideActivities[position].DurationMilliseconds);
                //string sDuration = duration.ToString(@"hh\:mm\:ss");

                outsideActivityViewHolder.OutsideActivityTextView.Text = _outsideActivitiesByMonth[position].StartTime.ToString("yyyy-MM-dd") + '\t' + (TimeSpan.FromMilliseconds(_outsideActivitiesByMonth[position].DurationMilliseconds)).ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
        
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.outside_activity_viewholder, parent, false);
            OutsideActivityViewHolder outsideActivityViewHolder = new OutsideActivityViewHolder(itemView);
            return outsideActivityViewHolder;
        }
    }

}