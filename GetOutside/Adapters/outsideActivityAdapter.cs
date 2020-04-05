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

    public class outsideActivityAdapter : RecyclerView.Adapter
    {
        private List<outsideActivity> _outsideActivities;
        private List<outsideActivity> _outsideActivitiesByMonth;
        public SqliteDataService _dataService = new SqliteDataService();
        public event EventHandler<int> ItemClick;

        public outsideActivityAdapter()
        {
            _dataService.Initialize();
            _outsideActivitiesByMonth = _dataService.GetOutsideHoursByMonth();
            //_outsideActivitiesByMonth = _dataService.GetOutsideHoursByDay();
            //_outsideActivitiesByMonth = _dataService.GetOutsideActivity();
        }

        public override int ItemCount => _outsideActivitiesByMonth.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is OutsideActivityViewHolder outsideActivityViewHolder)
            {
                //outsideActivityViewHolder.OutsideActivityTextView.Text = _outsideActivitiesByMonth[position].StartTime.ToString("yyyy-MM-dd") + "  " + (TimeSpan.FromMilliseconds(_outsideActivitiesByMonth[position].DurationMilliseconds)).ToString();
                outsideActivityViewHolder.OutsideActivityTextView.Text = _outsideActivitiesByMonth[position].YearMonth + "  " + (TimeSpan.FromMilliseconds(_outsideActivitiesByMonth[position].DurationMilliseconds)).ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
        
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.outside_activity_viewholder, parent, false);
            OutsideActivityViewHolder outsideActivityViewHolder = new OutsideActivityViewHolder(itemView, OnClick);
            return outsideActivityViewHolder;
        }

        private void OnClick(int obj)
        {
            ItemClick?.Invoke(this, obj);
        }
    }

}