using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using GetOutside.Adapters;

namespace GetOutside
{
    [Activity(Label = "outsideActivityAggregationActivity", Theme = "@style/AppTheme")]
    public class outsideActivityAggregationActivity : Activity
    {
        private RecyclerView _outsideActivityRecyclerView;
        private RecyclerView.LayoutManager _outsideActivityLayoutManager;
        private outsideActivityAggregationAdapter _outsideActivityAggregationAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.outside_activity_aggregation);
            _outsideActivityRecyclerView = FindViewById<RecyclerView>(Resource.Id.outsideActivityRecyclerView);

            _outsideActivityLayoutManager = new LinearLayoutManager(this);
            _outsideActivityRecyclerView.SetLayoutManager(_outsideActivityLayoutManager);
            _outsideActivityAggregationAdapter = new outsideActivityAggregationAdapter();
            _outsideActivityAggregationAdapter.ItemClick += _outsideActivityAggregationAdapter_ItemClick;
            _outsideActivityRecyclerView.SetAdapter(_outsideActivityAggregationAdapter);
        }

        private void _outsideActivityAggregationAdapter_ItemClick(object sender, int e)
        {
            var intent = new Intent();
            intent.SetClass(this, typeof(OutsideActivityDailyActivity));
            intent.PutExtra("selectedOutsideActivityId", e);
            StartActivity(intent);
        }
    }
}