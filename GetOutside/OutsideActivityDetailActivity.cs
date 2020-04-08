using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Content;
using GetOutside.Adapters;


namespace GetOutside
{
    [Activity(Label = "OutsideActivityDetailView", Theme = "@style/AppTheme")] 
    public class OutsideActivityDetailActivity : Activity
    {
        private RecyclerView _outsideActivityDetailRecyclerView;
        private RecyclerView.LayoutManager _outsideActivityLayoutManager;
        private outsideActivityDetailAdapter _outsideActivityDetailAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.outside_activity_detail);
            _outsideActivityDetailRecyclerView = FindViewById<RecyclerView>(Resource.Id.outsideActivityDetailRecyclerView);
//            _outsideActivityDetailRecyclerView = FindViewById<RecyclerView>(Resource.Id.outsideActivityDetailRecyclerView);

            _outsideActivityLayoutManager = new LinearLayoutManager(this);
            _outsideActivityDetailRecyclerView.SetLayoutManager(_outsideActivityLayoutManager);
            _outsideActivityDetailAdapter = new outsideActivityDetailAdapter();
            _outsideActivityDetailAdapter.ItemClick += _outsideActivityDetailAdapter_ItemClick;
            _outsideActivityDetailRecyclerView.SetAdapter(_outsideActivityDetailAdapter);
        }

        private void _outsideActivityDetailAdapter_ItemClick(object sender, int e)
        {
            //var intent = new Intent();
            //intent.SetClass(this, typeof(OutsideActivityDailyActivity));
            //intent.PutExtra("selectedOutsideActivityId", e);
            //StartActivity(intent);
        }
    }
}