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
        private List<View> viewList;
        private MainActivity mainActivity;

        public IntroPagerAdapter(List<View> mViewList, MainActivity mMainActivity)
        {
            this.viewList = mViewList;
            mainActivity = mMainActivity;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View view = viewList[position];
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
                    if (SharedService.isRoot())
                    {
                        new AlertDialog.Builder(mainActivity)
                                .SetTitle("危險")
                                .SetMessage("您的手機已 Root ，無法使用本程式")
                                .SetIcon(Resource.Drawable.Icon)
                                .SetNegativeButton("QQ", delegate
                                {
                                })
                                .Show();
                    }
                    else if (!SharedService.isFromGooglePlay(mainActivity))
                    {
                        new AlertDialog.Builder(mainActivity)
                                .SetTitle("警告")
                                .SetMessage("您並非使用 Google Play 安裝，無法使用本程式")
                                .SetIcon(Resource.Drawable.Icon)
                                .SetNegativeButton("QQ", delegate
                                {
                                })
                                .Show();
                    }
                    else
                    {
                        sp_Settings.Edit().PutBoolean("IsNeverIntro", ctv_NeverIntro.Checked).Apply();
                        mainActivity.SupportFragmentManager
                                .BeginTransaction()
                                .Replace(Resource.Id.MainFrameLayout, new HomeFragment(), "HomeFragment")
                                .Commit();
                    }
                };
            }
            container.AddView(view);
            return view;
        }

        public override int Count => viewList.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
    }
}