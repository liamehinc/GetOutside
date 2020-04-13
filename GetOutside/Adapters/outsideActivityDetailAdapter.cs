using System;
using Android.Support.V7.Widget;
using Android.Views;
using GetOutside.Core.Model;
using GetOutside.Database;
using System.Collections.Generic;
using GetOutside.ViewHolders;
using System.Globalization;

namespace GetOutside
{
    internal class outsideActivityDetailAdapter : RecyclerView.Adapter
    {
        private List<outsideActivity> _outsideActivities;
        private SqliteDataService _dataService = new SqliteDataService();
        public event EventHandler<int> ItemClick;

        public outsideActivityDetailAdapter()
        {
            _dataService.Initialize();
            _outsideActivities = _dataService.GetOutsideActivity();
 
        }

        public override int ItemCount => _outsideActivities.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is OutsideActivityDetailViewHolder outsideActivityViewHolder)
            {
                outsideActivityViewHolder.OutsideActivityDetailTextView.Text = _outsideActivities[position].StartTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + "  " + (TimeSpan.FromMilliseconds(_outsideActivities[position].DurationMilliseconds)).ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.outside_activity_detail_viewholder, parent, false);
            OutsideActivityDetailViewHolder outsideActivityDetailViewHolder = new OutsideActivityDetailViewHolder(itemView, OnClick);
            return outsideActivityDetailViewHolder;
        }

        private void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

    }
}