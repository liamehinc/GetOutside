using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Support.V7.Widget;
using Android.Views;
using GetOutside.Core.Model;
using GetOutside.Database;
using GetOutside.ViewHolders;

namespace GetOutside.Adapters
{
    public class outsideActivityAggregationAdapter : RecyclerView.Adapter
    {
        private List<outsideActivity> _outsideActivitiesByMonth;
        private SqliteDataService _dataService = new SqliteDataService();
        public event EventHandler<int> ItemClick;

        public outsideActivityAggregationAdapter()
        {
            _dataService.Initialize();
            _outsideActivitiesByMonth = _dataService.GetOutsideHoursByMonth();
        }

        public override int ItemCount => _outsideActivitiesByMonth.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is OutsideActivityAggregationViewHolder outsideActivityViewHolder)
            {
                outsideActivityViewHolder.OutsideActivityAggregationTextView.Text = _outsideActivitiesByMonth[position].StartTime.ToString("yyyy-MM", CultureInfo.CurrentCulture) + "  " + (TimeSpan.FromMilliseconds(_outsideActivitiesByMonth[position].DurationMilliseconds)).ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.outside_activity_aggregation_viewholder, parent, false);
            OutsideActivityAggregationViewHolder outsideActivityAggregationViewHolder = new OutsideActivityAggregationViewHolder(itemView, OnClick);
            return outsideActivityAggregationViewHolder;
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }

}