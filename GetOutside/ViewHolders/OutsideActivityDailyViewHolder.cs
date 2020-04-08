using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace GetOutside.ViewHolders
{
    public class OutsideActivityDailyViewHolder : RecyclerView.ViewHolder
    {
        public TextView OutsideActivityDailyTextView { get; internal set; }

        public OutsideActivityDailyViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            OutsideActivityDailyTextView = itemView.FindViewById<TextView>(Resource.Id.outsideActivityDailyTextView);
            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}