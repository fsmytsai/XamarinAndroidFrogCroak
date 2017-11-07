using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using FrogCroak.Views;

namespace FrogCroak.MyMethod
{
    public class IntroPagerAdapter : PagerAdapter
    {
        private List<View> mListViews;
        private MainActivity mainActivity;

        public IntroPagerAdapter(List<View> mListViews, Activity mActivity)
        {
            this.mListViews = mListViews;
            mainActivity = (MainActivity)mActivity;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = mListViews[position];
            if (position == 3)
            {

                ISharedPreferences sp_Settings = mainActivity.GetSharedPreferences("Settings", FileCreationMode.Private);
                CheckedTextView ctv_NeverIntro = (CheckedTextView)view.FindViewById(Resource.Id.ctv_NeverIntro);
                ctv_NeverIntro.Checked = sp_Settings.GetBoolean("IsNeverIntro", false);
                ctv_NeverIntro.Click += delegate
                {
                    ctv_NeverIntro.Toggle();
                };

                Button bt_StartConGroup = (Button)view.FindViewById(Resource.Id.bt_StartConGroup);
                bt_StartConGroup.Click += delegate
                {
                    sp_Settings.Edit().PutBoolean("IsNeverIntro", ctv_NeverIntro.Checked).Apply();
                    mainActivity.SupportFragmentManager
                            .BeginTransaction()
                            .Replace(Resource.Id.MainFrameLayout, new HomeFragment(), "HomeFragment")
                            .Commit();
                };
            }
            container.AddView(view);
            return view;
        }

        public override int Count => mListViews.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
    }
}