using Android.Views;
using Android.Widget;
using System;
using Android.Support.V7.Widget;

namespace GetOutside
{
    public class OutsideActivityDetailViewHolder : RecyclerView.ViewHolder
    {
        public TextView OutsideActivityDetailTextView { get; internal set; }

        public OutsideActivityDetailViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            OutsideActivityDetailTextView = itemView.FindViewById<TextView>(Resource.Id.outsideActivityDetailTextView);
            itemView.Click += (sender, e) => listener(LayoutPosition);
        }  
    }
}