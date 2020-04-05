using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace GetOutside.ViewHolders
{
    public class OutsideActivityViewHolder : RecyclerView.ViewHolder
    {
        public TextView OutsideActivityTextView { get; set;}

        public OutsideActivityViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            OutsideActivityTextView = itemView.FindViewById<TextView>(Resource.Id.outsideActivityTextView);
            ItemView.Click += (sender, e) => listener(Position);
        }
    }
}