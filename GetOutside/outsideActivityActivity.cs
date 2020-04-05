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
using GetOutside.Adapters;

namespace GetOutside
{
    [Activity(Label = "outsideActivityActivity")]
    public class outsideActivityActivity : Activity
    {
        private RecyclerView _outsideActivityRecyclerView;
        private RecyclerView.LayoutManager _outsideActivityLayoutManager;
        private outsideActivityAdapter _outsideActivityAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.outside_activity);
            _outsideActivityRecyclerView = FindViewById<RecyclerView>(Resource.Id.outsideActivityRecyclerView);

            _outsideActivityLayoutManager = new LinearLayoutManager(this);
            _outsideActivityRecyclerView.SetLayoutManager(_outsideActivityLayoutManager);
            _outsideActivityAdapter = new outsideActivityAdapter();
            _outsideActivityRecyclerView.SetAdapter(_outsideActivityAdapter);
        }

        void OnItemClick (object sender, int position)
        {
            int itemNum = position + 1;

        }
    }
}