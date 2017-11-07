using System;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Gms.Maps;
using FrogCroak.MyMethod;
using Android.Gms.Maps.Model;
using FrogCroak.Models;
using FrogCroak.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Android.Database;
using static FrogCroak.Models.MyMarkers;

namespace FrogCroak.Views
{
    public class MapFragment : Fragment
    {
        private MapView mMapView;
        public GoogleMap googleMap;
        private MainActivity mainActivity;

        private MyDBHelper helper;
        private ICursor cursor;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.MapFragment, container, false);
            mainActivity = (MainActivity)Activity;
            helper = new MyDBHelper(mainActivity, "frog.db", null, 1);
            mMapView = (MapView)view.FindViewById(Resource.Id.mapView);
            mMapView.OnCreate(savedInstanceState);

            mMapView.OnResume(); // needed to get the map to display immediately

            try
            {
                MapsInitializer.Initialize(Activity.ApplicationContext);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

            mMapView.GetMapAsync(new VeryNaiveImpl
            {
                Callback = (map) =>
                {
                    googleMap = map;
                    CustomInfoWindowAdapter adapter = new CustomInfoWindowAdapter(mainActivity, this);
                    googleMap.SetInfoWindowAdapter(adapter);

                    googleMap.Clear();
                    LatLng sydney = new LatLng(23.674764, 120.796819);
                    CameraPosition cameraPosition = new CameraPosition.Builder().Target(sydney).Zoom(7.3f).Build();
                    googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
                    DrawMap();
                }
            });
            return view;
        }

        public async void DrawMap()
        {
            if (mainActivity.sp_Settings.GetBoolean("IsShowMyMarker", true))
            {
                cursor = helper.ReadableDatabase.Query(
                    "marker", null,
                    null, null,
                    null, null, "_id DESC");
                LatLng sydney = new LatLng(0, 0);
                while (cursor.MoveToNext())
                {
                    double Latitude = cursor.GetDouble(1);
                    double Longitude = cursor.GetDouble(2);
                    string Title = cursor.GetString(3);
                    string Content = cursor.GetString(4);

                    sydney = new LatLng(Latitude, Longitude);
                    Marker marker = googleMap.AddMarker(new MarkerOptions()
                            .SetPosition(sydney)
                            .SetTitle(Title)
                            .SetSnippet(Content)
                            .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.personal))
                    );

                    marker.Tag = cursor.GetInt(0);
                }
            }

            if (mainActivity.sp_Settings.GetBoolean("IsShowAllMarker", true))
            {
                AllRequestResult result = null;
                await Task.Run(() =>
                {
                    result = new MarkerService().GetMarkerList();
                });
                if (result.IsSuccess)
                {
                    MyMarkers myMarkers = JsonConvert.DeserializeObject<MyMarkers>((string)result.Result);
                    LatLng sydney = new LatLng(0, 0);
                    foreach (MyMarker myMarker in myMarkers.MarkerList)
                    {
                        sydney = new LatLng(myMarker.Latitude, myMarker.Longitude);
                        Marker marker = googleMap.AddMarker(new MarkerOptions()
                                .SetPosition(sydney)
                                .SetTitle(myMarker.Title)
                                .SetSnippet(myMarker.Content.Replace("\\n", "\n"))
                                .SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.normal))
                        );
                    }
                    CameraPosition cameraPosition = new CameraPosition.Builder().Target(sydney).Zoom(7.3f).Build();
                    googleMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
                }
                else
                {
                    SharedService.ShowTextToast((string)result.Result, mainActivity);
                }
            }

        }
    }
}