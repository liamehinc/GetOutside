using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace GetOutside.ViewHolders
{
    public class OutsideActivityAggregationViewHolder : RecyclerView.ViewHolder
    {
        public TextView OutsideActivityAggregationTextView { get; internal set; }

        public OutsideActivityAggregationViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            OutsideActivityAggregationTextView = itemView.FindViewById<TextView>(Resource.Id.outsideActivityAggregationTextView);
            ItemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}