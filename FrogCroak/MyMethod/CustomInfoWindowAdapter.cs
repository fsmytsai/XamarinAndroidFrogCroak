using Android.App;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace FrogCroak.MyMethod
{
    public class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private Activity context;
        private Views.MapFragment mapFragment;

        public CustomInfoWindowAdapter(Activity context, Views.MapFragment mapFragment)
        {
            this.context = context;
            this.mapFragment = mapFragment;
        }

        public View GetInfoContents(Marker marker)
        {
            View view = context.LayoutInflater.Inflate(Resource.Layout.CustomInfoWindow, null);
            TextView tvTitle = (TextView)view.FindViewById(Resource.Id.tv_title);
            TextView tvSubTitle = (TextView)view.FindViewById(Resource.Id.tv_subtitle);
            tvTitle.Text = marker.Title;
            tvSubTitle.Text = marker.Snippet;
            //string tag = marker.Tag.ToString();
            //if (tag != "all")
            //{
            //    ImageView iv_Delete = view.FindViewById<ImageView>(Resource.Id.iv_Delete);
            //    iv_Delete.Visibility = ViewStates.Visible;
            //    iv_Delete.Click += delegate
            //    {
            //        MyDBHelper helper = new MyDBHelper(context, "frog.db", null, 1);
            //        helper.WritableDatabase.ExecSQL("DELETE FROM marker WHERE _id= " + tag);
            //        mapFragment.googleMap.Clear();
            //        mapFragment.DrawMap();
            //    };
            //}
            return view;
        }

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
    }
}