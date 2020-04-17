using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using GetOutside.Adapters;

namespace GetOutside
{
    [Activity(Label = "outsideActivityDailyActivity", Theme = "@style/AppTheme")]
    public class OutsideActivityDailyActivity : Activity
    {
        private RecyclerView _outsideActivityDailyRecyclerView;
        private RecyclerView.LayoutManager _outsideActivityLayoutManager;
        private outsideActivityDailyAdapter _outsideActivityDailyAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
           
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetContentView(Resource.Layout.outside_activity_daily);
            _outsideActivityDailyRecyclerView = FindViewById<RecyclerView>(Resource.Id.outsideActivityDailyRecyclerView);

            _outsideActivityLayoutManager = new LinearLayoutManager(this);
            _outsideActivityDailyRecyclerView.SetLayoutManager(_outsideActivityLayoutManager);
            _outsideActivityDailyAdapter = new outsideActivityDailyAdapter();
            _outsideActivityDailyAdapter.ItemClick += _outsideActivityDailyAdapter_ItemClick;
            _outsideActivityDailyRecyclerView.SetAdapter(_outsideActivityDailyAdapter);
        }
        private void _outsideActivityDailyAdapter_ItemClick(object sender, int e)
        {
            using (var intent = new Intent())
            {
                intent.SetClass(this, typeof(OutsideActivityDetailActivity));
                intent.PutExtra("selectedOutsideActivityId", e);
                StartActivity(intent);
            }
        }
    }
}