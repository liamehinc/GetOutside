using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class outsideActivityDailyAdapter : RecyclerView.Adapter
    {
        private List<outsideActivity> _outsideActivities;
        private SqliteDataService _dataService = new SqliteDataService();
        public event EventHandler<int> ItemClick;

        public outsideActivityDailyAdapter()
        {
            _dataService.Initialize();
            _outsideActivities = _dataService.GetOutsideHoursByDay();
        }

        public override int ItemCount => _outsideActivities.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is OutsideActivityDailyViewHolder outsideActivityViewHolder)
            {
                outsideActivityViewHolder.OutsideActivityDailyTextView.Text = _outsideActivities[position].StartTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + "  " + (TimeSpan.FromMilliseconds(_outsideActivities[position].DurationMilliseconds)).ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.outside_activity_daily_viewholder, parent, false);
            OutsideActivityDailyViewHolder outsideActivityDailyViewHolder = new OutsideActivityDailyViewHolder(itemView, OnClick);
            return outsideActivityDailyViewHolder;
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

    }
}